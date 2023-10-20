namespace JournalyApiV2.Models.Requests;

public class PatchMedsRequest
{
    public MedPatch[] Meds { get; set; } = Array.Empty<MedPatch>();
    public MedEntryPatch[] MedEntries { get; set; } = Array.Empty<MedEntryPatch>();

    public class MedPatch
    {
        public Guid Uuid { get; set; }
        public string? Name { get; set; }
        public string? Notes { get; set; }
        public short? Unit { get; set; }
        public short? DefaultDose { get; set; }
        public bool? Deleted { get; set; }
        public SchedulePatch[] Schedules { get; set; }
        public DateOnly? From { get; set; }
        public DateOnly? Until { get; set; }
        public bool? Forever { get; set; }
    }

    public class MedEntryPatch
    {
        
    }

    public class SchedulePatch
    {
        public TimeOnly Time { get; set; }
        public bool EveryOtherDay { get; set; }
        public short[] Days { get; set; } = Array.Empty<short>();
    }
}