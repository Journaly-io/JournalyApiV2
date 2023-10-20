using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("Day")]
public class Day
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public short Id { get; set; }
    
    [Column("Name")]
    public string Name { get; set; }
    
    // Navigation properties
    public ICollection<MedScheduleDays> MedScheduleDays { get; set; }
}