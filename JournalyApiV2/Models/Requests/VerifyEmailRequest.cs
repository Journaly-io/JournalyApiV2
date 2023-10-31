namespace JournalyApiV2.Models.Requests;

public class VerifyEmailRequest
{
    public string? LongCode { get; set; }
    public string? ShortCode { get; set; }
}