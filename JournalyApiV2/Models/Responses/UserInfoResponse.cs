namespace JournalyApiV2.Models.Responses;

public class UserInfoResponse
{
    public Guid Uuid { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }
    public bool HasRecoveryKeys { get; set; }
    public string EncryptedDEK { get; set; }
    public string KEKSalt { get; set; }
}