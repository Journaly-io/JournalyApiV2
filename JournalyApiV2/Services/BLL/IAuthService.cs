using JournalyApiV2.Models.Responses;

namespace JournalyApiV2.Services.BLL;

public interface IAuthService
{
    Task CreateUser(string email, string password, string firstName, string lastName);
    Task<AuthenticationResponse> SignIn(string email, string password);
    Task VoidToken(string token);
    Task ChangeName(string firstName, string lastName, Guid userId);
    Task ChangeEmail(string email, Guid userId, int tokenId);
    Task ChangePassword(Guid userId, string oldPassword, string newPassword, int tokenId, bool signOutEverywhere = true);
    Task VerifyEmail(Guid userId, string toEmail, string firstName, string lastName);
    Task VerifyEmailWithLongCode(string longCode);
    Task VerifyEmailWithShortCode(Guid userId, string shortCode);
    Task ResendVerificationEmailAsync(Guid userId);
    Task ResetPasswordAsync(string email);
    Task SubmitPasswordResetAsync(string code, string password, bool signOutEverywhere = true);
    Task SignOutEverywhereAsync(Guid userId, int? tokenId);
}