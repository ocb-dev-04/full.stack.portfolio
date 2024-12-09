using Services.Doctors.Domain.Entities;
using Shared.Common.Helper.ErrorsHandler;
using Services.Doctors.Domain.Abstractions;
using CQRS.MediatR.Helper.Abstractions.Messaging;
using Services.Doctors.Domain.Errors;
using Services.Doctors.Domain.StrongIds;

namespace Services.Doctors.Application.UseCases;

internal sealed class RemoveDoctorCommandHandler
    : ICommandHandler<RemoveDoctorCommand>
{
    private readonly IDoctorRepository _doctorRepository;

    public RemoveDoctorCommandHandler(IDoctorRepository doctorRepository)
    {
        ArgumentNullException.ThrowIfNull(doctorRepository, nameof(doctorRepository));

        _doctorRepository = doctorRepository;
    }

    public async Task<Result> Handle(RemoveDoctorCommand request, CancellationToken cancellationToken)
    {
        Result<DoctorId> doctorId = DoctorId.Create(request.Id);
        if (doctorId.IsFailure)
            return Result.Failure(doctorId.Error);

        Result<Doctor> found = await _doctorRepository.ByIdAsync(doctorId.Value, cancellationToken);
        if (found.IsFailure)
            return Result.Failure(found.Error);

        if (!found.Value.CredentialId.Value.Equals(request.CredentialId))
            return Result.Failure(DoctorErrors.YouAreNotTheOwner);

        await _doctorRepository.DeleteAsync(found.Value.Id, cancellationToken);
        await _doctorRepository.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
