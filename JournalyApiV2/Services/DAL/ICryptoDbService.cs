using JournalyApiV2.Data.Models;
using EncryptedDEKType = JournalyApiV2.Data.Enums.EncryptedDEKType;

namespace JournalyApiV2.Services.DAL;

public interface ICryptoDbService
{
    Task StoreNewDEKForUser(Guid user, string DEK, string salt, EncryptedDEKType type);
    Task<EncryptedDEK[]> GetDEKsForUser(Guid user, EncryptedDEKType[] typeFilter = null);
}