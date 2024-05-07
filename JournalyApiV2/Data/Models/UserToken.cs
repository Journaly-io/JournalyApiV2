using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("UserToken")]
public class UserToken
{
    [Column("Token")]
    public string Token { get; set; }

    [Column("UserId")]
    public Guid UserId;
}