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

    public async Task PatchJournal(PatchJournalRequest request, Guid owner, Guid deviceId)
    {

        await _journalDbService.SyncCategories(request.Categories, owner, deviceId); // Categories must come before emotions and activities due to database constraints
            await Task.WhenAll(
                _journalDbService.SyncEmotions(request.Emotions, owner, deviceId),
                _journalDbService.SyncActivities(request.Activities, owner, deviceId)
            );
            await _journalDbService.SyncJournalEntries(request.JournalEntries, owner, deviceId) // Must come last due to database constraints
        );
    }
}