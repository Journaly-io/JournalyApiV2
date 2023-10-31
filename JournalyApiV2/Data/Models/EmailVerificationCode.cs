using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("EmailVerificationCode")]
public class EmailVerificationCode
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("ShortCode")]
    [StringLength(6)]
    public string ShortCode { get; set; }
    
    [Column("LongCode")]
    public string LongCode { get; set; }
    
    [Column("User")]
    public Guid User { get; set; }
}