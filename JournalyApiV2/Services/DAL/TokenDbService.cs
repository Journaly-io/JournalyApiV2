using System.Security.Cryptography;
using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;

namespace JournalyApiV2.Services.DAL;

public class TokenDbService : ITokenDbService
{
    private readonly IDbFactory _db;

    public TokenDbService(IDbFactory db)
    {
        _db = db;
    }


    public async Task<string> GenerateToken(Guid userId)
    {
        await using var db = _db.Journaly();
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        var randomNumber = new byte[32];
        randomNumberGenerator.GetBytes(randomNumber);
        var token = Convert.ToBase64String(randomNumber);
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
}