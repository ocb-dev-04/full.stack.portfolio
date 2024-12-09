using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Patients.Application.UseCases;

internal sealed class GetPatientByIdQueryHandler
    : IQueryHandler<GetPatientByIdQuery, IEnumerable<PatientResponse>>
{
    public Task<Result<IEnumerable<PatientResponse>>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
