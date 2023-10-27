using Microsoft.AspNetCore.Identity;

namespace JournalyApiV2.Models;

public class JournalyUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public override string UserName 
    { 
        get => Email; 
        set => Email = value; 
    }
}