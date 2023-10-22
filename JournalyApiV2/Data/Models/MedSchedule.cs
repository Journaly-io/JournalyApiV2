using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("MedSchedule")]
public class MedSchedule
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("Time")]
    public TimeOnly Time { get; set; }
    
    [Column("EveryOtherDay")]
    public bool EveryOtherDay { get; set; }
    
    [Column("MedicationUuid")]
    [ForeignKey("Medication")]
    public Guid MedicationUuid { get; set; }
    
    // Navigation properties
    public Medication Medication { get; set; }
    public ICollection<MedScheduleDays> Days { get; set; }
    public ICollection<MedicationInstance> Instances { get; set; }
}