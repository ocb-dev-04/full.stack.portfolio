﻿using Common.Services.Bus.Abstractions;
using MassTransit;
using MediatR;
using Services.Patients.Application.UseCases;
using Shared.Common.Helper.ErrorsHandler;
using Shared.Common.Helper.Extensions;
using Shared.Common.Helper.Models.QueueResponses;
using Shared.Message.Queue.Requests.Requests;
using Shared.Message.Queue.Requests.Responses;

namespace Services.Patients.Application.Consumers;

internal sealed class GetPatientCollectionByDoctorIdsConsumer
    : IConsumer<GetPatientCollectionByDoctorIdRequest>
{
    #region Props & ctor

    private readonly ISender _sender;
    private readonly IExecuteHandlerService _executeHandlerRepository;

    public GetPatientCollectionByDoctorIdsConsumer(
        ISender sender,
        IExecuteHandlerService executeHandlerRepository)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(executeHandlerRepository, nameof(executeHandlerRepository));

        _sender = sender;
        _executeHandlerRepository = executeHandlerRepository;
    }

    #endregion
    public async Task Consume(ConsumeContext<GetPatientCollectionByDoctorIdRequest> context)
        => await _executeHandlerRepository.Execute(() => Process(context), context);

    private async Task Process(ConsumeContext<GetPatientCollectionByDoctorIdRequest> context)
    {
        GetPatientCollectionByDoctorIdQuery query = new(context.Message.Id);
        Result<IEnumerable<PatientResponse>> queryResponse = await _sender.Send(query, context.CancellationToken);

        if (queryResponse.IsFailure)
        {
            BusMessageResponse failureResponse = new BusMessageResponse().Failed(queryResponse.Error.Translation, queryResponse.Error.Description);
            BusMessageResult failureResult = new(failureResponse.Serialize());
            await context.RespondAsync<BusMessageResult>(failureResult);
            return;
        }

        IEnumerable<PatientQueueResponse> collection = queryResponse.Value.Select(s 
            => PatientQueueResponse.Map(s.Id, s.Name, s.Age, s.CreatedOnUtc));
        string serialize = new PatientCollectionQueueResponse(collection).Serialize();

        BusMessageResponse response = new BusMessageResponse().Done(serialize);
        BusMessageResult result = new(response.Serialize());
        await context.RespondAsync<BusMessageResult>(result);
    }
}