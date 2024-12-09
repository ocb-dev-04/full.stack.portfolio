using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Patients.Application.UseCases;

internal sealed class RemovePatientCommandHandler
    : ICommandHandler<RemovePatientCommand>
{
    public Task<Result> Handle(RemovePatientCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
