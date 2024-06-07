using JournalyApiV2.Models.Requests;
using JournalyApiV2.Models.Responses;

namespace JournalyApiV2.Services.BLL;

public interface IAuthService
{
    Task<AuthenticationResponse> SignIn(string email, string password);
    Task VoidToken(string token);
    Task CreateUser(string email, string password, string firstName, string lastName, string encryptedDEK, string KEKSalt);
    Task ChangeName(string firstName, string lastName, Guid userId);
    Task ChangeEmail(string email, string passwordHash, Guid userId);
    Task ChangePassword(Guid userId, string oldPassword, string newPassword, string encryptedDEK, string KEKSalt, string initiatorToken, bool signOutEverywhere = true);
    Task VerifyEmail(Guid userId, string toEmail, string firstName, string lastName);
    Task VerifyEmailWithLongCode(string longCode);
    Task VerifyEmailWithShortCode(Guid userId, string shortCode);
    Task ResendVerificationEmailAsync(Guid userId);
    Task SignOutEverywhereAsync(Guid userId, string initiatorToken);
    Task<UserInfoResponse> GetUserInfoAsync(Guid userId);
    Task BeginAccountRecovery(string email);
    Task<string> IssueRecoveryTokenWithShortCode(string email, string shortCode);
    Task<string> IssueRecoveryTokenWithLongCode(string longCode);
    Task<CryptographicKey[]> GetRecoveryKeys(string recoveryToken);
}