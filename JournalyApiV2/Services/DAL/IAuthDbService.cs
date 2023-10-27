namespace JournalyApiV2.Services.DAL;

public interface IAuthDbService
{
    Task<string?> ExchangeRefreshTokenAsync(string token);
    Task<Guid?> LookupRefreshTokenAsync(string token);
    Task<string?> NewRefreshTokenAsync(Guid user);
}