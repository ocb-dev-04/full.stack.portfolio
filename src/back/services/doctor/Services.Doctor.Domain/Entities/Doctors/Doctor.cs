using Services.Auth.Domain.StrongIds;
using Value.Objects.Helper.Values.Complex;
using Value.Objects.Helper.Values.Primitives;

namespace Services.Auth.Domain.Entities;

public sealed partial class Doctor
{
    public DoctorId Id { get; init; }
    public GuidObject IdAsGuid { get; init; }
    public GuidObject CredentialId { get; init; }

    public StringObject Name { get; private set; }
    public IntegerObject Age { get; private set; }
    public StringObject Specialty { get; private set; }

    public AuditDates AuditDates { get; init; } = AuditDates.Init();

    private Doctor()
    {

    }

    private Doctor(
        GuidObject credentialId,
        StringObject name,
        IntegerObject age,
        StringObject specialty)
    {
        Id = DoctorId.New();
        IdAsGuid = GuidObject.Create(Id.Value.ToString());
        CredentialId = credentialId;

        Name = name;
        Age = age;
        Specialty = specialty;
    }
}
