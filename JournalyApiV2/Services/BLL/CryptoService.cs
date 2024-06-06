using JournalyApiV2.Data.Models;
using JournalyApiV2.Services.DAL;
using EncryptedDEKType = JournalyApiV2.Data.Enums.EncryptedDEKType;

namespace JournalyApiV2.Services.BLL;

public class CryptoService : ICryptoService
{
    private readonly ICryptoDbService _cryptoDbService;

    public CryptoService(ICryptoDbService cryptoDbService)
    {
        _cryptoDbService = cryptoDbService;
    }

    public async Task StoreNewDEKForUser(Guid user, string DEK, string salt, EncryptedDEKType type)
    {
        await _cryptoDbService.StoreNewDEKForUser(user, DEK, salt, type);
    }

    public async Task<EncryptedDEK[]> GetDEKsForUser(Guid user, EncryptedDEKType[]? typeFilter = null)
    {
        return await _cryptoDbService.GetDEKsForUser(user, typeFilter);
    }
}