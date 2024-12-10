using Shared.Common.Helper.ErrorsHandler;
using Services.Diagnoses.Domain.Entities;
using Services.Diagnoses.Domain.Abstractions;
using Value.Objects.Helper.Values.Primitives;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Service.Diagnoses.Application.UseCases;

internal sealed class GetDiagnosisByIdQueryHandler
    : IQueryHandler<GetDiagnosisByIdQuery, DiagnosisResponse>
{
    private readonly IDiagnosisRepository _diagnosisRepository;

    public GetDiagnosisByIdQueryHandler(IDiagnosisRepository diagnosisRepository)
    {
        ArgumentNullException.ThrowIfNull(diagnosisRepository, nameof(diagnosisRepository));

        _diagnosisRepository = diagnosisRepository;
    }

    public async Task<Result<DiagnosisResponse>> Handle(GetDiagnosisByIdQuery request, CancellationToken cancellationToken)
    {
        Result<Diagnosis> found = await _diagnosisRepository.ByIdAsync(GuidObject.Create(request.Id.ToString()), cancellationToken);
        if (found.IsFailure)
            return Result.Failure<DiagnosisResponse>(found.Error);

        return DiagnosisResponse.Map(found.Value);
    }
}
