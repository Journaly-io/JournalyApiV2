namespace JournalyApiV2.Services.DAL;

public interface ITokenDbService
{
    Task<string> GenerateToken(Guid userId);
    Task<Guid?> ValidateToken(string token);
    Task RevokeToken(string token);
}