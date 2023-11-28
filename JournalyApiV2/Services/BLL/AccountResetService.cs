namespace JournalyApiV2.Services.BLL;

public class AccountResetService : IAccountResetService
{
    private readonly IMedService _medService;

    public AccountResetService(IMedService medService)
    {
        _medService = medService;
    }

    public async Task AccountResetAsync(Guid user)
    {
        await _medService.ClearMeds(user);
    }
}