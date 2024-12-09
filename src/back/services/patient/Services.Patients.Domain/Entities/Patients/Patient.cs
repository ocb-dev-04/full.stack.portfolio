using Services.Patients.Domain.StrongIds;
using Value.Objects.Helper.Values.Complex;
using Value.Objects.Helper.Values.Primitives;

namespace Services.Patients.Domain.Entities;

public sealed partial class Patient
{
    public PatientId Id { get; init; }
    public GuidObject IdAsGuid { get; init; }
    public GuidObject DoctorId { get; init; }

    public StringObject Name { get; private set; }
    public IntegerObject Age { get; private set; }
    public BooleanObject Deleted { get; private set; } = BooleanObject.CreateAsFalse();

    public AuditDates AuditDates { get; init; } = AuditDates.Init();

    private Patient()
    {

    }

    private Patient(
        GuidObject doctorId,
        StringObject name,
        IntegerObject age)
    {
        Id = PatientId.New();
        IdAsGuid = GuidObject.Create(Id.Value.ToString());
        DoctorId = doctorId;

        Name = name;
        Age = age;
    }
}
