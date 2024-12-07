using MassTransit;

namespace Shared.Message.Queue.Requests.Requests;

[EntityName("get-diagnosis-by-id-bind")]
public sealed class GetDiagnosisByIdRequest
{
    public Guid Id { get; set; }

    public GetDiagnosisByIdRequest()
    {

    }

    public static GetDiagnosisByIdRequest Create(Guid id)
        => new GetDiagnosisByIdRequest { Id = id };
}