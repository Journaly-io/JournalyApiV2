namespace JournalyApiV2.Models.Requests;

public class ResetPasswordRequest
{
    public string Code { get; set; }
    public string NewPassword { get; set; }
}