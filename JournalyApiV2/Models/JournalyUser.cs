using Microsoft.AspNetCore.Identity;

namespace JournalyApiV2.Models;

public class JournalyUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EncryptedDEK { get; set; }
    public string KEKSalt { get; set; }
}