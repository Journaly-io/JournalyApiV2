using JournalyApiV2.Data.Models;
using JournalyApiV2.Models.Requests;
using JournalyApiV2.Services.DAL;
using Microsoft.AspNetCore.Authorization;

namespace JournalyApiV2.Services.BLL;

[Authorize]
public class JournalService : IJournalService
{
    private readonly IJournalDbService _journalDbService;

    public JournalService(IJournalDbService journalDbService)
    {
        _journalDbService = journalDbService;
    }

    public async Task PatchJournal(PatchJournalRequest request, Guid owner)
    {
        await Task.WhenAll(
            _journalDbService.SyncCategories(request.Categories, owner),
            _journalDbService.SyncEmotions(request.Emotions, owner)
        );
    }
}