using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;
using Services.Diagnoses.Application.Services;
using Services.Diagnoses.Domain.Abstractions;

namespace Service.Diagnoses.Application.UseCases;

internal sealed class RemovePatientCommandHandler
    : ICommandHandler<RemovePatientCommand>
{
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly MessageQeueServices _messageQeueServices;

    public RemovePatientCommandHandler(
        IDiagnosisRepository diagnosisRepository,
        MessageQeueServices messageQeueServices)
    {
        ArgumentNullException.ThrowIfNull(diagnosisRepository, nameof(diagnosisRepository));
        ArgumentNullException.ThrowIfNull(messageQeueServices, nameof(messageQeueServices));

        _diagnosisRepository = diagnosisRepository;
        _messageQeueServices = messageQeueServices;
    }

    public Task<Result> Handle(RemovePatientCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
