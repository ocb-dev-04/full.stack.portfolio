using Services.Auth.Domain.StrongIds;
using Value.Objects.Helper.Values.Domain;
using Value.Objects.Helper.Values.Complex;
using Value.Objects.Helper.Values.Primitives;

namespace Services.Auth.Domain.Entities;

public sealed partial class Credential
{
    public CredentialId Id { get; init; }
    public GuidObject IdAsGuid { get; init; }
    public EmailAddress Email { get; init; }

    public StringObject Password { get; private set; }
    public StringObject PrivateKey { get; private set; }

    public AuditDates AuditDates { get; init; } = AuditDates.Init();

    private Credential()
    {

    }

    private Credential(
        EmailAddress email,
        StringObject passwordHash)
    {
        Id = CredentialId.New();
        IdAsGuid = GuidObject.Create(Id.Value.ToString());
        Email = email;

        Password = passwordHash;
    }
}
