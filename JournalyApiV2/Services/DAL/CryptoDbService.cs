using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using JournalyApiV2.Pipeline;
using Microsoft.EntityFrameworkCore;
using EncryptedDEKType = JournalyApiV2.Data.Enums.EncryptedDEKType;

namespace JournalyApiV2.Services.DAL;

public class CryptoDbService : ICryptoDbService
{
    private readonly IDbFactory _db;

    public CryptoDbService(IDbFactory db)
    {
        _db = db;
    }

    public async Task StoreNewDEKForUser(Guid user, string DEK, string salt, EncryptedDEKType type)
    {
        await using var db = _db.Journaly();
        if (type == EncryptedDEKType.Hardware)  // Hardware keys/passkeys can have up to 3 instances
        {
            var currentHardwareKeyCount = (await GetDEKsForUser(user, new[] { EncryptedDEKType.Hardware })).Length;
            if (currentHardwareKeyCount >= 3) throw new HttpBadRequestException("Too many hardware keys");
        }
        else
        {
            var currentCountOfThisType = (await GetDEKsForUser(user, new[] { type })).Length;
            if (currentCountOfThisType > 0) throw new HttpBadRequestException("Too many keys of this type");
        }

        db.EncryptedDeks.Add(new EncryptedDEK
        {
            Owner = user,
            EncryptedDEKTypeId = (short)type,
            DEK = DEK,
            Salt = salt
        });
        await db.SaveChangesAsync();
    }

    public async Task UpdateDEKForUser(Guid user, string DEK, string salt, EncryptedDEKType type)
    {
        await using var db = _db.Journaly();
        if (type == EncryptedDEKType.Hardware)
            throw new ArgumentException("Hardware keys are not supported by this method as there may be multiple");

        var dek = await db.EncryptedDeks.SingleAsync(x => x.Owner == user && x.EncryptedDEKTypeId == (short)type);
        dek.DEK = DEK;
        dek.Salt = salt;
        await db.SaveChangesAsync();
    }

    public async Task<EncryptedDEK[]> GetDEKsForUser(Guid user, EncryptedDEKType[]? typeFilter = null)
    {
        await using var db = _db.Journaly();
        if (typeFilter == null)
            return await db.EncryptedDeks.Where(x => x.Owner == user).ToArrayAsync();
        return await db.EncryptedDeks.Where(x => x.Owner == user && typeFilter.Select(y => (int)y).Contains(x.EncryptedDEKTypeId))
            .ToArrayAsync();
    }
}