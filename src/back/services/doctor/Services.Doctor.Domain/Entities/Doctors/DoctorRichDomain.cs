using Value.Objects.Helper.Values.Primitives;

namespace Services.Auth.Domain.Entities;

public sealed partial class Doctor
{
    /// <summary>
    /// Create a new <see cref="Doctor"/>
    /// </summary>
    /// <param name="credentialId"></param>
    /// <param name="name"></param>
    /// <param name="age"></param>
    /// <param name="specialty"></param>
    /// <returns></returns>
    public static Doctor Create(
        GuidObject credentialId,
        StringObject name,
        IntegerObject age,
        StringObject specialty)
    {
        Doctor created = new(credentialId, name, age, specialty);

        return created;
    }

    /// <summary>
    /// Update <see cref="Doctor"/> general info
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public void Update(
        StringObject name,
        IntegerObject age,
        StringObject specialty)
    {
        Name = name;
        Age = age;
        Specialty = specialty;

        AuditDates.ChangesApplied();
    }
}
