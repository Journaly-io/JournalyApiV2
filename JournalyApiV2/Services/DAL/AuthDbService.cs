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

}