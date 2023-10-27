using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("UserId")]
    public Guid UserId { get; set; }
    
    [Column("Token")]
    public string Token { get; set; }
}