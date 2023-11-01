using System.Security.Cryptography;
using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using Microsoft.EntityFrameworkCore;
using RefreshToken = JournalyApiV2.Data.Models.RefreshToken;

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

    public async Task<Models.RefreshToken?> ExchangeRefreshTokenAsync(string token)
    {
        await using var db = _db.Journaly();
        var oldToken = await db.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token);
        if (oldToken == null) return null;
        db.Remove(oldToken);
        var newToken = GenerateSecureOpaqueToken();
        var result = db.RefreshTokens.Add(new RefreshToken
        {
            Token = newToken,
            UserId = oldToken.UserId
        });
        await db.SaveChangesAsync();
        return new Models.RefreshToken
        {
            Token = newToken,
            TokenId = result.Entity.Id
        };
    }

    public async Task<Models.RefreshToken> NewRefreshTokenAsync(Guid user)
    {
        await using var db = _db.Journaly();
        var newToken = GenerateSecureOpaqueToken();
        var result = db.RefreshTokens.Add(new RefreshToken
        {
            Token = newToken,
            UserId = user
        });
        await db.SaveChangesAsync();
        return new Models.RefreshToken
        {
            Token = newToken,
            TokenId = result.Entity.Id
        };
    }

    public async Task<Guid?> LookupRefreshTokenAsync(string token)
    {
        await using var db = _db.Journaly();
        var result = await db.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token);
        return result?.UserId;
    }

    public async Task VoidRefreshTokensAsync(params int[] tokenIds)
    {
        await using var db = _db.Journaly();
        // Use the Contains method to find tokens whose Id is in the tokenIds array.
        var tokens = db.RefreshTokens.Where(up => tokenIds.Contains(up.Id));
        db.RemoveRange(tokens);
        await db.SaveChangesAsync();
    }

    public async Task<Models.RefreshToken[]> GetRefreshTokensAsync(Guid userId)
    {
        await using var db = _db.Journaly();
        return db.RefreshTokens.Where(x => x.UserId == userId).Select(x => new Models.RefreshToken
        {
            Token = x.Token,
            TokenId = x.Id
        }).ToArray();
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
            throw new TooEarlyException($"Too early to resend: Please wait an additional {Convert.ToInt16((code.LastSent.AddSeconds(60) - DateTime.UtcNow).TotalSeconds)} seconds");
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
        var code = await db.EmailVerificationCodes.SingleOrDefaultAsync(x => x.User == userId && x.ShortCode == shortCode);

        return code != null;
    }
}