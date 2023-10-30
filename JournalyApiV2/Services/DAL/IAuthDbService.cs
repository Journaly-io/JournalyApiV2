using JournalyApiV2.Models;

namespace JournalyApiV2.Services.DAL;

public interface IAuthDbService
{
    Task<RefreshToken?> ExchangeRefreshTokenAsync(string token);
    Task<Guid?> LookupRefreshTokenAsync(string token);
    Task<RefreshToken> NewRefreshTokenAsync(Guid user);
    Task VoidRefreshTokensAsync(params int[] tokenIds);
    Task<RefreshToken[]> GetRefreshTokensAsync(Guid userId);
}