using JournalyApiV2.Data.Models;
using JournalyApiV2.Models.Requests;
using JournalyApiV2.Services.DAL;

namespace JournalyApiV2.Services.BLL;

public class JournalService : IJournalService
{
    private readonly IJournalDbService _journalDbService;

    public JournalService(IJournalDbService journalDbService)
    {
        _journalDbService = journalDbService;
    }

    public async Task PatchJournal(PatchJournalRequest request)
    {
        await Task.WhenAll(
            _journalDbService.SyncCategories(request.Categories)
        );
    }
}