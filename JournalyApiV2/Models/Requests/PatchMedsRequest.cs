using JournalyApiV2.Data.Enums;

namespace JournalyApiV2.Models.Requests;

public class PatchMedsRequest
{
    public MedPatch[] Meds { get; set; } = Array.Empty<MedPatch>();
    public MedInstancePatch[] MedInstances { get; set; } = Array.Empty<MedInstancePatch>();
    public SchedulePatch[] Schedules { get; set; }

    public class MedPatch
    {
        public Guid Uuid { get; set; }
        public string? Name { get; set; }
        public string? Notes { get; set; }
        public short? Unit { get; set; }
        public short? DefaultDose { get; set; }
        public bool? Deleted { get; set; }
        public DateOnly? From { get; set; }
        public DateOnly? Until { get; set; }
        public bool? Forever { get; set; }
    }

    public class MedInstancePatch
    {
        public Guid Uuid { get; set; }
        public Guid? MedicationUuid { get; set; }
        public Guid? ScheduleUuid { get; set; }
        public short? Dose { get; set; }
        public DateTime? ScheduledTime { get; set; } // This can actually be set to null so it must be specified every time
        public DateTime? ActualTime { get; set; } // This can actually be set to null so it must be specified every time
        public MedStatus? Status { get; set; }
        public bool? Deleted { get; set; }
    }

    public class SchedulePatch
    {
        public Guid Uuid { get; set; }
        public TimeOnly Time { get; set; }
        public bool EveryOtherDay { get; set; }
        public short[] Days { get; set; } = Array.Empty<short>();
    }
}