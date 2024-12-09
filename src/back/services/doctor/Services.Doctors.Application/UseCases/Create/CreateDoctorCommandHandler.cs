using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;
using Services.Doctors.Domain.Abstractions;
using Shared.Common.Helper.Providers;
using Shared.Common.Helper.Models;
using Value.Objects.Helper.Values.Primitives;
using Services.Doctors.Domain.Errors;
using Services.Doctors.Domain.Entities;

namespace Services.Doctors.Application.UseCases;

internal sealed class CreateDoctorCommandHandler
    : ICommandHandler<CreateDoctorCommand, DoctorResponse>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly HttpRequestProvider _httpRequestProvider;

    public CreateDoctorCommandHandler(
        IDoctorRepository doctorRepository, 
        HttpRequestProvider httpRequestProvider)
    {
        ArgumentNullException.ThrowIfNull(doctorRepository, nameof(doctorRepository));
        ArgumentNullException.ThrowIfNull(httpRequestProvider, nameof(httpRequestProvider));

        _doctorRepository = doctorRepository;
        _httpRequestProvider = httpRequestProvider;
    }

    public async Task<Result<DoctorResponse>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        Result<CurrentRequestUser> currentUser = _httpRequestProvider.GetContextCurrentUser();
        if (currentUser.IsFailure)
            return Result.Failure<DoctorResponse>(currentUser.Error);

        StringObject name = StringObject.Create(request.Name);
        StringObject specialty = StringObject.Create(request.Specialty);
        bool exist = await _doctorRepository.ExistAsync(e => e.Name.Equals(name) && e.Specialty.Equals(specialty), cancellationToken);
        if (exist)
            return Result.Failure<DoctorResponse>(DoctorErrors.AlreadyExist);

        Doctor created = Doctor.Create(
            GuidObject.Create(currentUser.Value.CredentialId.ToString()),
            name,
            specialty,
            IntegerObject.Create(request.ExperienceInYears));

        await _doctorRepository.CreateAsync(created, cancellationToken);
        await _doctorRepository.CommitAsync(cancellationToken);

        return DoctorResponse.Map(created);
    }
}
