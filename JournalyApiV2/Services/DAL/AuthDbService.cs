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
    
    public async Task<string> UpdateRefreshTokenAsync(Guid user)
    {
        await using var db = _db.Journaly();
        db.RemoveRange(db.RefreshTokens.Where(x => x.UserId == user));
        var token = GenerateSecureOpaqueToken();
        await db.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user,
            Token = token
        });
        await db.SaveChangesAsync();
        return token;
    }

    public async Task<Guid?> LookupRefreshTokenAsync(string token)
    {
        await using var db = _db.Journaly();
        var result = await db.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token);
        return result?.UserId;
    }
}