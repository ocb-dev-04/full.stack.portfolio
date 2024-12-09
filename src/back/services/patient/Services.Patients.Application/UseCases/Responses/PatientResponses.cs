using Services.Patients.Domain.Entities;

namespace Services.Patients.Application.UseCases;

public sealed record PatientResponse(
    Guid Id,
    string Name,
    int Age,
    DateTimeOffset CreatedOnUtc)
{
    public static PatientResponse Map(Patient entity)
        => new(
            entity.Id.Value,
            entity.Name.Value,
            entity.Age.Value,
            entity.AuditDates.CreatedOnUtc);
}
