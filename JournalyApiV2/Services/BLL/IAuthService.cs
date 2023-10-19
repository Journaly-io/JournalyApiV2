namespace JournalyApiV2.Services.BLL;

public interface IAuthService
{
    Task CreateUser(string email, string password, string firstName, string lastName);
}