using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Patients.Application.UseCases;

public sealed class CreatePatientCommandHandler
    : ICommandHandler<CreatePatientCommand, PatientResponse>
{
    public Task<Result<PatientResponse>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
