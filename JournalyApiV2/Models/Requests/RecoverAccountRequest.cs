namespace JournalyApiV2.Models.Requests;

public class RecoverAccountRequest
{
    public string DEK { get; set; }
    public string Salt { get; set; }
    public string passwordHash { get; set; }
}