namespace JournalyApiV2.Models.Responses;

public class SignInResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
}