using Services.Doctors.Domain.Entities;
using Shared.Common.Helper.ErrorsHandler;
using Services.Doctors.Domain.Abstractions;
using Value.Objects.Helper.Values.Primitives;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Doctors.Application.UseCases;

internal sealed class GetDoctorCollectionBySpecialtyQueryHandler
    : IQueryHandler<GetDoctorCollectionBySpecialtyQuery, IEnumerable<DoctorResponse>>
{
    private readonly IDoctorRepository _doctorRepository;

    public GetDoctorCollectionBySpecialtyQueryHandler(IDoctorRepository doctorRepository)
    {
        ArgumentNullException.ThrowIfNull(doctorRepository, nameof(doctorRepository));

        _doctorRepository = doctorRepository;
    }

    public async Task<Result<IEnumerable<DoctorResponse>>> Handle(GetDoctorCollectionBySpecialtyQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Doctor> collection = await _doctorRepository.CollectionBySpecialtyAsync(StringObject.Create(request.Specialty), request.PageNumber, cancellationToken);
        IEnumerable<DoctorResponse> mapped = collection.Select(s => DoctorResponse.Map(s));

        return Result.Success(mapped);
    }
}
