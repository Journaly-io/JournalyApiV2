namespace JournalyApiV2.Services.BLL;

public interface IAccountResetService
{
    Task AccountResetAsync(Guid user);
}