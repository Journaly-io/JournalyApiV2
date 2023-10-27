using JournalyApiV2.Models.Responses;

namespace JournalyApiV2.Services.BLL;

public interface IAuthService
{
    Task CreateUser(string email, string password, string firstName, string lastName);
    Task<SignInResponse> SignIn(string email, string password);
    Task<SignInResponse> RefreshToken(string refreshToken);
}