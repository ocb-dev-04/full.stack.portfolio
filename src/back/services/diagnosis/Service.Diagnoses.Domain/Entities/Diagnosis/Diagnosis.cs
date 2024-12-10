using Value.Objects.Helper.Values.Complex;
using Value.Objects.Helper.Values.Primitives;

namespace Service.DiagnosesDomain.Entities;

public sealed partial class Diagnosis
{
    public GuidObject Id { get; init; }
    public GuidObject DoctorId { get; init; }
    public GuidObject PatientId { get; init; }

    public StringObject Disease { get; private set; }
    public StringObject Medicine { get; private set; }
    public StringObject Indications { get; private set; }
    public TimeSpan DosageInterval { get; set; }

    public AuditDates AuditDates { get; init; } = AuditDates.Init();

    private Diagnosis()
    {

    }

    private Diagnosis(
        GuidObject doctorId,
        GuidObject patientId,
        StringObject disease,
        StringObject medicine,
        StringObject indications,
        TimeSpan dosageInterval)
    {
        Id = GuidObject.New();
        DoctorId = doctorId;
        PatientId = patientId;

        Disease = disease;
        Medicine = medicine;
        Indications = indications;
        DosageInterval = dosageInterval;
    }
}
