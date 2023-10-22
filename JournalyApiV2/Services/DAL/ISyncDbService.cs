using JournalyApiV2.Models;

namespace JournalyApiV2.Services.DAL;

public interface ISyncDbService
{
    Task MarkSynced(params RecordSync[] recordSyncs);
    Task<Models.JournalEntry[]> GetUnsyncedJournalEntries(Guid userGuid, Guid deviceGuid);
    Task<Models.Emotion[]> GetUnsyncedEmotions(Guid userGuid, Guid deviceGuid);
    Task<Models.Activity[]> GetUnsyncedActivities(Guid userGuid, Guid deviceGuid);
    Task<Models.EmotionCategory[]> GetUnsyncedEmotionCategories(Guid userGuid, Guid deviceGuid);
    Task<Models.Medication[]> GetUnsyncedMedications(Guid userGuid, Guid deviceGuid);
    Task<Models.Schedule[]> GetUnsyncedSchedules(Guid userGuid, Guid deviceGuid);
}