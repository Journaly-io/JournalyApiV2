using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Requests;
using JournalyApiV2.Pipeline;
using Microsoft.EntityFrameworkCore;
using EncryptedDEKType = JournalyApiV2.Data.Enums.EncryptedDEKType;

namespace JournalyApiV2.Services.DAL;

public class AuthDbService : IAuthDbService
{
    private readonly IDbFactory _db;

    public AuthDbService(IDbFactory db)
    {
        _db = db;
    }

    private static string GenerateSecureOpaqueToken()
    {
        using var rng = new RNGCryptoServiceProvider();
        byte[] byteArr = new byte[32];
        rng.GetBytes(byteArr);
        return Convert.ToBase64String(byteArr);
    }

    private static string GenerateSecureShortCode()
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] buffer = new byte[4];

            rng.GetBytes(buffer);
            int result = BitConverter.ToInt32(buffer, 0);

            return Math.Abs(result % 1000000).ToString("D6");
        }
    }

    public async Task<EmailVerification> GetOrCreateEmailVerificationCode(Guid userId)
    {
        await using var db = _db.Journaly();
        var code = await db.EmailVerificationCodes.SingleOrDefaultAsync(x => x.User == userId);

        if (code != null)
            return new EmailVerification
            {
                ShortCode = code.ShortCode,
                LongCode = code.LongCode
            };

        var shortCode = GenerateSecureShortCode();
        var longCode = GenerateSecureOpaqueToken();

        await db.EmailVerificationCodes.AddAsync(new EmailVerificationCode
        {
            User = userId,
            ShortCode = shortCode,
            LongCode = longCode
        });
        await db.SaveChangesAsync();

        return new EmailVerification
        {
            ShortCode = shortCode,
            LongCode = longCode
        };
    }

    public async Task ResetEmailVerificationTimerAsync(Guid userId)
    {
        await using var db = _db.Journaly();
        var code = await db.EmailVerificationCodes.SingleOrDefaultAsync(x => x.User == userId);
        if (code == null) throw new ArgumentException("No user verification found for given ID");
        if (code.LastSent.AddSeconds(60) >= DateTime.UtcNow)
            throw new TooEarlyException(
                $"Too early to resend: Please wait an additional {Convert.ToInt16((code.LastSent.AddSeconds(60) - DateTime.UtcNow).TotalSeconds)} seconds");
        code.LastSent = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task<Guid?> GetUserByLongCode(string longCode)
    {
        await using var db = _db.Journaly();
        return (await db.EmailVerificationCodes.SingleOrDefaultAsync(x => x.LongCode == longCode))?.User;
    }

    public async Task VerifyUser(Guid user)
    {
        await using var db = _db.Journaly();
        var toRemove = db.EmailVerificationCodes.Where(x => x.User == user);
        db.RemoveRange(toRemove);
        await db.SaveChangesAsync();
    }

    public async Task<bool> CheckShortCode(Guid userId, string shortCode)
    {
        await using var db = _db.Journaly();
        var code = await db.EmailVerificationCodes.SingleOrDefaultAsync(x =>
            x.User == userId && x.ShortCode == shortCode);
        return code != null;
    }

    public async Task ClearEmailVerificationCodes(Guid userId)
    {
        await using var db = _db.Journaly();
        db.RemoveRange(db.EmailVerificationCodes.Where(x => x.User == userId));
        await db.SaveChangesAsync();
    }

public async Task<string?> GetPasswordResetCode(Guid userId)
    {
        await using var db = _db.Journaly();
        var code = await db.PasswordResetCodes.SingleOrDefaultAsync(x => x.User == userId);
        return code?.Code;
    }

    public async Task<string> GeneratePasswordResetCode(Guid userId)
    {
        await using var db = _db.Journaly();
        var code = await db.PasswordResetCodes.SingleOrDefaultAsync(x => x.User == userId);
        if (code != null) return code.Code;

        code = new PasswordResetCode
        {
            Code = GenerateSecureOpaqueToken(),
            LastSent = DateTime.UtcNow,
            User = userId
        };

        await db.PasswordResetCodes.AddAsync(code);
        await db.SaveChangesAsync();

        return code.Code;
    }

    public async Task ResetPasswordResetTimerAsync(Guid userId)
    {
        await using var db = _db.Journaly();
        var code = await db.PasswordResetCodes.SingleOrDefaultAsync(x => x.User == userId);
        if (code == null) throw new ArgumentException("User not found");
        if (code.LastSent.AddSeconds(60) >= DateTime.UtcNow)
            throw new TooEarlyException(
                $"Too early to send another reset, please try again in {Convert.ToInt16((code.LastSent.AddSeconds(60) - DateTime.UtcNow).TotalSeconds)} seconds");
        code.LastSent = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }
    
    public async Task<Guid?> LookupPasswordResetAsync(string code)
    {
        await using var db = _db.Journaly();
        var codeObj = await db.PasswordResetCodes.SingleOrDefaultAsync(x => x.Code == code);

        return codeObj?.User;
    }

    public async Task ResetPassword(Guid userId)
    {
        await using var db = _db.Journaly();
        var toRemove = db.PasswordResetCodes.Where(x => x.User == userId);
        db.RemoveRange(toRemove);
        await db.SaveChangesAsync();
    }
    
    public async Task<string> GenerateToken(Guid userId)
    {
        await using var db = _db.Journaly();
        var token = GenerateSecureOpaqueToken();
        db.UserTokenStore.Add(new UserToken
        {
            Token = token,
            UserId = userId,
        });
        await db.SaveChangesAsync();
        return token;
    }

    public async Task<Guid?> ValidateToken(string token)
    {
        await using var db = _db.Journaly();
        var userToken = db.UserTokenStore.FirstOrDefault(t => t.Token == token);
        return userToken?.UserId;
    }

    public async Task RevokeToken(string token)
    {
        await using var db = _db.Journaly();
        var userToken = db.UserTokenStore.FirstOrDefault(t => t.Token == token);
        if (userToken != null)
        {
            db.UserTokenStore.Remove(userToken);
            await db.SaveChangesAsync();
        }
    }
    
    public async Task RevokeTokens(Guid userId, string[]? exclude)
    {
        if (exclude == null) exclude = Array.Empty<string>();
        await using var db = _db.Journaly();
        var userTokens = await db.UserTokenStore.Where(t => userId == t.UserId && !exclude.Contains(t.Token)).ToListAsync();
        if (userTokens.Count > 0)
        {
            db.UserTokenStore.RemoveRange(userTokens);
            await db.SaveChangesAsync();
        }
    }

    public async Task<string> IssueRecoveryToken(Guid userId)
    {
        await using var db = _db.Journaly();
        var token = GenerateSecureOpaqueToken();
        db.AccountRecoveryTokens.Add(new AccountRecoveryToken
        {
            Token = token,
            UserId = userId
        });
        await db.SaveChangesAsync();
        return token;
    }

    public async Task<CryptographicKey[]> GetRecoveryKeys(string recoveryToken)
    {
        await using var db = _db.Journaly();
        var userId = (await db.AccountRecoveryTokens.SingleOrDefaultAsync(x => x.Token == recoveryToken))?.UserId;
        if (userId == null) throw new HttpBadRequestException("Invalid recovery token");
        var dbDeks = await db.EncryptedDeks.Where(x => x.EncryptedDEKTypeId != (int)Data.Enums.EncryptedDEKType.Primary)
            .ToArrayAsync();
        return dbDeks.Select(x => new CryptographicKey
        {
            DEK = x.DEK,
            Salt = x.Salt,
            Type = x.EncryptedDEKTypeId
        }).ToArray();
    }
 
    public async Task<CryptographicKey[]> GetRecoveryKeys(Guid userId)
    {
        await using var db = _db.Journaly();
        return (await db.EncryptedDeks.Where(x => x.EncryptedDEKTypeId != (short)EncryptedDEKType.Primary && x.Owner == userId)
            .ToArrayAsync()).Select(x => new CryptographicKey
            {
                DEK = x.DEK,
                Salt = x.Salt,
                Type = (int)x.Type.Id
            }).ToArray();
    }

    public async Task ClearRecoveryTokens(Guid userId)
    {
        await using var db = _db.Journaly();
        db.RemoveRange(db.AccountRecoveryTokens.Where(x => x.UserId == userId));
        await db.SaveChangesAsync();
    }

    public async Task<Guid?> GetUserIdFromRecoveryToken(string recoveryToken)
    {
        await using var db = _db.Journaly();

        var recoveryTokenObj = await db.AccountRecoveryTokens.SingleOrDefaultAsync(x => x.Token == recoveryToken);

        return recoveryTokenObj?.UserId;
    }
}