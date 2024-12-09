using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Patients.Application.UseCases;

internal sealed class UpdatePatientCommandHandler
    : ICommandHandler<UpdatePatientCommand, PatientResponse>
{
    public Task<Result<PatientResponse>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
