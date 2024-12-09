using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;
using Services.Doctors.Domain.Abstractions;
using Services.Doctors.Domain.Entities;
using Services.Doctors.Domain.Errors;
using Shared.Common.Helper.Models;
using Shared.Common.Helper.Providers;
using Value.Objects.Helper.Values.Primitives;

namespace Services.Doctors.Application.UseCases;
internal sealed class UpdateDoctorCommandHandler
    : ICommandHandler<UpdateDoctorCommand, DoctorResponse>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly HttpRequestProvider _httpRequestProvider;

    public UpdateDoctorCommandHandler(
        IDoctorRepository doctorRepository,
        HttpRequestProvider httpRequestProvider)
    {
        ArgumentNullException.ThrowIfNull(doctorRepository, nameof(doctorRepository));
        ArgumentNullException.ThrowIfNull(httpRequestProvider, nameof(httpRequestProvider));

        _doctorRepository = doctorRepository;
        _httpRequestProvider = httpRequestProvider;
    }

    public async Task<Result<DoctorResponse>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        Result<CurrentRequestUser> currentUser = _httpRequestProvider.GetContextCurrentUser();
        if (currentUser.IsFailure)
            return Result.Failure<DoctorResponse>(currentUser.Error);

        Result<Doctor> found = await _doctorRepository.ByCredentialId(GuidObject.Create(currentUser.Value.CredentialId.ToString()), cancellationToken);
        if (found.IsFailure)
            return Result.Failure<DoctorResponse>(found.Error);

        found.Value.UpdateGeneaalData(
            StringObject.Create(request.Name),
            StringObject.Create(request.Specialty),
            IntegerObject.Create(request.ExperienceInYears));
        await _doctorRepository.CommitAsync(cancellationToken);

        return DoctorResponse.Map(found.Value);
    }
}
