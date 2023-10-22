using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("MedStatus")]
public class MedStatus
{
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public short Id { get; set; }
    
    [Column("Name")]
    public string Name { get; set; }
}