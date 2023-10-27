namespace JournalyApiV2.Services.DAL;

public interface IAuthDbService
{
    Task<string> UpdateRefreshTokenAsync(Guid user);
    Task<Guid?> LookupRefreshTokenAsync(string token);
}