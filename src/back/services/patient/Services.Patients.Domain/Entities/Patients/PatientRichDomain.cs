using Shared.Common.Helper.Extensions;
using Value.Objects.Helper.Values.Primitives;

namespace Services.Patients.Domain.Entities;

public sealed partial class Patient
{
    /// <summary>
    /// Create a new <see cref="Patient"/>
    /// </summary>
    /// <param name="credentialId"></param>
    /// <param name="name"></param>
    /// <param name="specialty"></param>
    /// <param name="experienceInYears"></param>
    /// <returns></returns>
    public static Patient Create(
        GuidObject credentialId,
        StringObject name,
        StringObject specialty,
        IntegerObject experienceInYears)
    {
        Patient created = new(
            credentialId, 
            name, 
            name.Value.NormalizeToFTS(), 
            specialty, experienceInYears);

        return created;
    }

    /// <summary>
    /// Update <see cref="Patient"/> general information
    /// </summary>
    /// <param name="name"></param>
    /// <param name="specialty"></param>
    /// <param name="experienceInYears"></param>
    public void UpdateGeneaalData(
        StringObject name,
        StringObject specialty,
        IntegerObject experienceInYears)
    {
        Name = name;
        Specialty = specialty;
        ExperienceInYears = experienceInYears;

        AuditDates.ChangesApplied();
    }
}
