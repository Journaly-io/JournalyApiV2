using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("MedScheduleDays")]
public class MedScheduleDays
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("MedScheduleId")]
    [ForeignKey("MedSchedule")]
    public int MedScheduleId { get; set; }
    
    [Column("Day")]
    [ForeignKey("Day")]
    public short DayId { get; set; }
    
    // Navigation Properties
    public MedSchedule MedSchedule { get; set; }
    public Day Day { get; set; }
}