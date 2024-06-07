namespace JournalyApiV2.Models.Requests;

public class VerifyEmailRequest
{
    public string? LongCode { get; set; }
    public string? ShortCode { get; set; }
    public string? Email { get; set; } // Only used for email verification in the case of account recovery initiation
}