using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JournalyApiV2.Data.Enums;

namespace JournalyApiV2.Data.Models;

[Table("EncryptedDEK")]
public class EncryptedDEK
{
    [Column("Id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("Owner")]
    public Guid Owner { get; set; }
    
    [Column]
    public string Salt { get; set; }
    
    [Column("Type")]
    [ForeignKey("EncrypteDEKType")]
    public short EncryptedDEKTypeId { get; set; }
    
    // Navigation properties
    public EncryptedDEKType Type { get; set; }
}