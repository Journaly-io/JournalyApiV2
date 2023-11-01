using JournalyApiV2.Models;

namespace JournalyApiV2.Services.DAL;

public interface IAuthDbService
{
    Task<Models.RefreshToken?> ExchangeRefreshTokenAsync(string token);
    Task<Models.RefreshToken> NewRefreshTokenAsync(Guid user);
    Task<Guid?> LookupRefreshTokenAsync(string token);
    Task VoidRefreshTokensAsync(params int[] tokenIds);
    Task<Models.RefreshToken[]> GetRefreshTokensAsync(Guid userId);
    Task<EmailVerification> GetOrCreateEmailVerificationCode(Guid userId);
    Task ResetEmailVerificationTimerAsync(Guid userId);
    Task<Guid?> GetUserByLongCode(string longCode);
    Task VerifyUser(Guid user);
    Task<bool> CheckShortCode(Guid userId, string shortCode);
    Task<string?> GetPasswordResetCode(Guid userId);
    Task<string> GeneratePasswordResetCode(Guid userId);
    Task ResetPasswordResetTimerAsync(Guid userId);
    Task<Guid?> LookupPasswordResetAsync(string code);
    Task ResetPassword(Guid userId);
}