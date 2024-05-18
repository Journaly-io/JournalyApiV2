namespace JournalyApiV2.Models.Requests;

public class ChangeEmailRequest
{
    public string Email { get; set; }
    public string KEKSalt { get; set; }
    public string EncryptedDEK { get; set; }
}