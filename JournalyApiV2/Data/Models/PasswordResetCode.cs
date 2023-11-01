using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("PasswordResetCode")]
public class PasswordResetCode
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("Code")]
    public string Code { get; set; }
    
    [Column("User")]
    public Guid User { get; set; }
    
    [Column("LastSent")]
    public DateTime LastSent { get; set; }
}