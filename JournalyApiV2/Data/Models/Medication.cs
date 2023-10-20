using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("Medication")]
public class Medication
{
    [Key]
    [Column("Uuid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Uuid { get; set; }
    
    [Column("Name")]
    public string Name { get; set; }
    
    [Column("Notes")]
    public string Notes { get; set; }
    
    [Column("Unit")]
    [ForeignKey("MedUnit")]
    public short Unit { get; set; }
    
    [Column("DefaultDose")]
    public short DefaultDose { get; set; }
    
    [Column("From")]
    public DateOnly FromDate { get; set; }
    
    [Column("Until")]
    public DateOnly UntilDate { get; set; }
    
    [Column("Forever")]
    public bool Forever { get; set; }
    
    [Column("Deleted")]
    public bool Deleted { get; set; }
    
    // Navigation properties
    public MedUnit MedUnit { get; set; }
    public ICollection<MedSchedule> MedSchedules { get; set; }
}