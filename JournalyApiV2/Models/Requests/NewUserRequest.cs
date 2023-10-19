namespace JournalyApiV2.Models.Requests;

public class NewUserRequest
{
    public string Password { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}