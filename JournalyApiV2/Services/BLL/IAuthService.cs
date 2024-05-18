using JournalyApiV2.Models.Responses;

namespace JournalyApiV2.Services.BLL;

public interface IAuthService
{
    Task CreateUser(string email, string password, string firstName, string lastName, string encryptedDEK,
        string KEKSalt);
    Task<AuthenticationResponse> SignIn(string email, string password);
    Task VoidToken(string token);
    Task ChangeName(string firstName, string lastName, Guid userId);
    Task ChangeEmail(string email, string passwordHash, Guid userId);

    Task ChangePassword(Guid userId, string oldPassword, string newPassword, string encryptedDEK, string KEKSalt,
        string initiatorToken, bool signOutEverywhere = true);
    Task VerifyEmail(Guid userId, string toEmail, string firstName, string lastName);
    Task VerifyEmailWithLongCode(string longCode);
    Task VerifyEmailWithShortCode(Guid userId, string shortCode);
    Task ResendVerificationEmailAsync(Guid userId);
    Task SignOutEverywhereAsync(Guid userId, string initiatorToken);
    Task<UserInfoResponse> GetUserInfoAsync(Guid userId);
    Task UpdateKEK(Guid userId, string wrappedDEK, string KEKSalt);
}