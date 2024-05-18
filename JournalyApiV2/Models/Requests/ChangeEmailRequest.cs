namespace JournalyApiV2.Models.Requests;

public class ChangeEmailRequest
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}