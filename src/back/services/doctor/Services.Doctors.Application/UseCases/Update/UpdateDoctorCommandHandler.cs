using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;
using Services.Doctors.Domain.Abstractions;
using Services.Doctors.Domain.Entities;
using Services.Doctors.Domain.Errors;
using Shared.Common.Helper.Models;
using Shared.Common.Helper.Providers;
using Value.Objects.Helper.Values.Primitives;
using Services.Doctors.Domain.StrongIds;

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
        Result<DoctorId> doctorId = DoctorId.Create(request.Id); 
        if (doctorId.IsFailure)
            return Result.Failure<DoctorResponse>(doctorId.Error);

        Result<Doctor> found = await _doctorRepository.ByIdAsync(doctorId.Value, cancellationToken);
        if (found.IsFailure)
            return Result.Failure<DoctorResponse>(found.Error);

        if(!found.Value.CredentialId.Value.Equals(request.CredentialId))
            return Result.Failure<DoctorResponse>(DoctorErrors.YouAreNotTheOwner);

        found.Value.UpdateGeneaalData(
            StringObject.Create(request.Body.Name),
            StringObject.Create(request.Body.Specialty),
            IntegerObject.Create(request.Body.ExperienceInYears));
        await _doctorRepository.CommitAsync(cancellationToken);

        return DoctorResponse.Map(found.Value);
    }
}
