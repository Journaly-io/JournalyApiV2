namespace JournalyApiV2.Models.Requests;

public class ChangePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public bool SignOutEverywhere { get; set; } = true;
    public string encryptedDEK { get; set; }
    public string KEKSalt { get; set; }
}