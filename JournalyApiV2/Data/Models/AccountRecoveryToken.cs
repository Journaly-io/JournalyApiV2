using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("AccountRecoveryToken")]
public class AccountRecoveryToken
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("Token")]
    public string Token { get; set; }
    
    [Column]
    public Guid UserId { get; set; }
}