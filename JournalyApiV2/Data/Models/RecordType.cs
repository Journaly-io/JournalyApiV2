using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("RecordType")]
public class RecordType
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }
    
    [Column("Name")]
    public string Name { get; set; }
    
    // Navigation properties
    public ICollection<SyncedRecords> SyncedRecords { get; set; }
}