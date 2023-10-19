using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("SyncedRecords")]
public class SyncedRecords
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("DeviceId")]
    public Guid DeviceId { get; set; }
    
    [Column("RecordId")]
    public Guid RecordId { get; set; }
    
    [Column("RecordType")]
    [ForeignKey("RecordType")]
    public short RecordTypeId { get; set; }
    
    [Column("Timestamp")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime Timestamp { get; set; }
    
    // Navigation properties
    public RecordType RecordType { get; set; }
}