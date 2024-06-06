using JournalyApiV2.Data.Models;
using EncryptedDEKType = JournalyApiV2.Data.Enums.EncryptedDEKType;

namespace JournalyApiV2.Services.BLL;

public interface ICryptoService
{
    Task StoreNewDEKForUser(Guid user, string DEK, string salt, EncryptedDEKType type);
    Task<EncryptedDEK[]> GetDEKsForUser(Guid user, EncryptedDEKType[]? typeFilter = null);
}