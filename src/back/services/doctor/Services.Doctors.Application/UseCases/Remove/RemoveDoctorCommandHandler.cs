using Shared.Common.Helper.Models;
using Shared.Common.Helper.Providers;
using Services.Doctors.Domain.Entities;
using Shared.Common.Helper.ErrorsHandler;
using Services.Doctors.Domain.Abstractions;
using Value.Objects.Helper.Values.Primitives;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Doctors.Application.UseCases;

internal sealed class RemoveDoctorCommandHandler
    : ICommandHandler<RemoveDoctorCommand>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly HttpRequestProvider _httpRequestProvider;

    public RemoveDoctorCommandHandler(
        IDoctorRepository doctorRepository,
        HttpRequestProvider httpRequestProvider)
    {
        ArgumentNullException.ThrowIfNull(doctorRepository, nameof(doctorRepository));
        ArgumentNullException.ThrowIfNull(httpRequestProvider, nameof(httpRequestProvider));

        _doctorRepository = doctorRepository;
        _httpRequestProvider = httpRequestProvider;
    }

    public async Task<Result> Handle(RemoveDoctorCommand request, CancellationToken cancellationToken)
    {
        Result<CurrentRequestUser> currentUser = _httpRequestProvider.GetContextCurrentUser();
        if (currentUser.IsFailure)
            return Result.Failure(currentUser.Error);

        Result<Doctor> found = await _doctorRepository.ByCredentialId(GuidObject.Create(currentUser.Value.CredentialId.ToString()), cancellationToken);
        if (found.IsFailure)
            return Result.Failure(found.Error);

        await _doctorRepository.DeleteAsync(found.Value.Id, cancellationToken);
        await _doctorRepository.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
