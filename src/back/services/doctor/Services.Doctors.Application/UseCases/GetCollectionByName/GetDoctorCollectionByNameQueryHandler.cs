using Services.Doctors.Domain.Entities;
using Shared.Common.Helper.ErrorsHandler;
using Services.Doctors.Domain.Abstractions;
using Value.Objects.Helper.Values.Primitives;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Doctors.Application.UseCases;

internal sealed class GetDoctorCollectionByNameQueryHandler
    : IQueryHandler<GetDoctorCollectionByNameQuery, IEnumerable<DoctorResponse>>
{
    private readonly IDoctorRepository _doctorRepository;

    public GetDoctorCollectionByNameQueryHandler(IDoctorRepository doctorRepository)
    {
        ArgumentNullException.ThrowIfNull(doctorRepository, nameof(doctorRepository));

        _doctorRepository = doctorRepository;
    }

    public async Task<Result<IEnumerable<DoctorResponse>>> Handle(GetDoctorCollectionByNameQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Doctor> collection = await _doctorRepository.CollectionByNameAsync(StringObject.Create(request.Name), request.PageNumber, cancellationToken);
        IEnumerable<DoctorResponse> mapped = collection.Select(s => DoctorResponse.Map(s));

        return Result.Success(mapped);
    }
}
