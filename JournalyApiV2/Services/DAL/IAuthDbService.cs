using JournalyApiV2.Models;

namespace JournalyApiV2.Services.DAL;

public interface IAuthDbService
{
    Task<EmailVerification> GetOrCreateEmailVerificationCode(Guid userId);
    Task ResetEmailVerificationTimerAsync(Guid userId);
    Task<Guid?> GetUserByLongCode(string longCode);
    Task VerifyUser(Guid user);
    Task<bool> CheckShortCode(Guid userId, string shortCode);
    Task ClearEmailVerificationCodes(Guid userId);
    Task<string?> GetPasswordResetCode(Guid userId);
    Task<string> GeneratePasswordResetCode(Guid userId);
    Task ResetPasswordResetTimerAsync(Guid userId);
    Task<Guid?> LookupPasswordResetAsync(string code);
    Task ResetPassword(Guid userId);
    Task<string> GenerateToken(Guid userId);
    Task<Guid?> ValidateToken(string token);
    Task RevokeToken(string token);
    Task RevokeTokens(Guid userId, string[]? exclude);
    Task<string> IssueRecoveryToken(Guid userId);
    Task<Guid?> SpendRecoveryToken(string recoveryToken);
}