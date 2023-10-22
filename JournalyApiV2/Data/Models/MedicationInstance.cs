using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JournalyApiV2.Data.Enums;

namespace JournalyApiV2.Data.Models;

[Table("MedicationInstance")]
public class MedicationInstance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Uuid")]
    public Guid Uuid { get; set; }
    
    [Column("MedicationUuid")]
    [ForeignKey("Medication")]
    public Guid MedicationUuid { get; set; } // Cannot always be implied from schedule UUID due to unscheduled doses
    
    [Column("ScheduleUuid")]
    [ForeignKey("MedSchedule")]
    public Guid? ScheduleUuid { get; set; } // Can be null if unscheduled
    
    [Column("Dose")]
    public short Dose { get; set; }
    
    [Column("ScheduledTime")]
    public DateTime? ScheduledTime { get; set; } // Can be null if unscheduled
    
    [Column("ActualTime")]
    public DateTime? ActualTime { get; set; } // Can be null if scheduled but not taken yet
    
    [Column("MedStatus")]
    [ForeignKey("MedStatus")]
    public Enums.MedStatus Status { get; set; }

    [Column("Deleted")] 
    public bool Deleted { get; set; }
    
    [Column("Owner")]
    public Guid Owner { get; set; }
    
    // Navigation properties
    public Medication Medication { get; set; }
    public MedSchedule Schedule { get; set; }
}