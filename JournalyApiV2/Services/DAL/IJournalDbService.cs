using JournalyApiV2.Models;
using JournalyApiV2.Models.Requests;

namespace JournalyApiV2.Services.DAL;

public interface IJournalDbService
{
    Task SyncCategories(PatchJournalRequest.CategoryPatch[] categories, Guid owner, Guid deviceId);
    Task SyncEmotions(PatchJournalRequest.EmotionPatch[] emotions, Guid owner, Guid deviceId);
    Task SyncActivities(PatchJournalRequest.ActivityPatch[] activities, Guid owner, Guid deviceId);
    Task SyncJournalEntries(PatchJournalRequest.JournalPatch[] entries, Guid owner, Guid deviceId);
}