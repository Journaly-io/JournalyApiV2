using System.Security.Cryptography;
using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using Microsoft.EntityFrameworkCore;

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
    
    public async Task<string?> ExchangeRefreshTokenAsync(string token)
    {
        await using var db = _db.Journaly();
        var oldToken = await db.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token);
        if (oldToken == null) return null;
        db.Remove(oldToken);
        var newToken = GenerateSecureOpaqueToken();
        db.RefreshTokens.Add(new RefreshToken
        {
            Token = newToken,
            UserId = oldToken.UserId
        });
        await db.SaveChangesAsync();
        return newToken;
    }

    public async Task<string?> NewRefreshTokenAsync(Guid user)
    {
        await using var db = _db.Journaly();
        var newToken = GenerateSecureOpaqueToken();
        db.RefreshTokens.Add(new RefreshToken
        {
            Token = newToken,
            UserId = user
        });
        await db.SaveChangesAsync();
        return newToken;
    }

    public async Task<Guid?> LookupRefreshTokenAsync(string token)
    {
        await using var db = _db.Journaly();
        var result = await db.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token);
        return result?.UserId;
    }
}