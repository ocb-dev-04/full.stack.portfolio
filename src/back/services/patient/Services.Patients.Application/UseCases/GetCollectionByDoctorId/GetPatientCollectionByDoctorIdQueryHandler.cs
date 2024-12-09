using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Patients.Application.UseCases;

internal sealed class GetPatientCollectionByDoctorIdQueryHandler
    : IQueryHandler<GetPatientCollectionByDoctorIdQuery, IEnumerable<PatientResponse>>
{
    public Task<Result<IEnumerable<PatientResponse>>> Handle(GetPatientCollectionByDoctorIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
