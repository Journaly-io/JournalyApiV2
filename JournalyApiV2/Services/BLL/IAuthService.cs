using JournalyApiV2.Models.Responses;

namespace JournalyApiV2.Services.BLL;

public interface IAuthService
{
    Task CreateUser(string email, string password, string firstName, string lastName);
    Task<AuthenticationResponse> SignIn(string email, string password);
    Task VoidToken(int tokenId);
    Task<AuthenticationResponse> RefreshToken(string refreshToken);
    Task<AuthenticationResponse> ChangeName(string firstName, string lastName, Guid userId, int tokenId);
    Task<AuthenticationResponse> ChangeEmail(string email, Guid userId, int tokenId);
    Task ChangePassword(Guid userId, string oldPassword, string newPassword, int tokenId);
    Task VerifyEmail(Guid userId, string toEmail, string firstName, string lastName);
    Task VerifyEmailWithLongCode(string longCode);
    Task VerifyEmailWithShortCode(Guid userId, string shortCode);
}