using MassTransit;

namespace Shared.Message.Queue.Requests.Requests;

[EntityName("get-diagnosis-collection-by-patient-id-bind")]
public sealed class GetDiagnosisCollectionByPatientIdRequest
{
    public Guid Id { get; set; }

    public GetDiagnosisCollectionByPatientIdRequest()
    {

    }

    public static GetDiagnosisCollectionByPatientIdRequest Create(Guid id)
        => new GetDiagnosisCollectionByPatientIdRequest { Id = id };
}