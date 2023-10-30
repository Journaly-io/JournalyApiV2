using JournalyApiV2.Data.Models;

namespace JournalyApiV2.Models;

public class MedInstance
{
    public Guid Uuid { get; set; }
    public Guid MedicationUuid { get; set; } // Cannot always be implied from the ScheduleUuid because of unscheduled doses
    public Guid? ScheduleUuid { get; set; } // Can be null if unscheduled
    public short Dose { get; set; }
    public DateTime? ScheduledTime { get; set; } // Can be null if unscheduled
    public DateTime? ActualTime { get; set; } // Can be null if scheduled but not taken yet
    public bool Deleted { get; set; }
    public Data.Enums.MedStatus Status { get; set; }
}