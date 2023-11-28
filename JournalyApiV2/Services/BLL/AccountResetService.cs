namespace JournalyApiV2.Services.BLL;

public class AccountResetService : IAccountResetService
{
    private readonly IMedService _medService;
    private readonly IJournalService _journalService;

    public AccountResetService(IMedService medService, IJournalService journalService)
    {
        _medService = medService;
        _journalService = journalService;
    }

    public async Task AccountResetAsync(Guid user)
    {
        await _medService.ClearMeds(user);
        await _journalService.ClearJournal(user);
    }
}