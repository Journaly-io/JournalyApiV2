using JournalyApiV2.Data.Enums;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Responses;
using JournalyApiV2.Services.DAL;

namespace JournalyApiV2.Services.BLL;

public class SyncService : ISyncService
{
    private readonly ISyncDbService _syncDbService;

    public SyncService(ISyncDbService syncDbService)
    {
        _syncDbService = syncDbService;
    }

    public async Task<SyncMedResponse> GetUnsyncedMedData(Guid userGuid, Guid deviceGuid)
    {
        var medsTask = Task.Run(() => _syncDbService.GetUnsyncedMedications(userGuid, deviceGuid));

        await Task.WhenAll(medsTask);

        var meds = medsTask.Result;

        var medSyncs = meds.Select(x => new RecordSync
        {
            DeviceId = deviceGuid,
            RecordId = x.Uuid,
            RecordType = RecordType.Med
        });

        var recordSyncs = medSyncs.ToArray();

        await _syncDbService.MarkSynced(recordSyncs);

        return new SyncMedResponse
        {
            Medications = meds
        };
    }
    
    public async Task<SyncJournalResponse> GetUnsyncedJournalData(Guid userGuid, Guid deviceGuid)
    {
        var journalEntriesTask = Task.Run(() => _syncDbService.GetUnsyncedJournalEntries(userGuid, deviceGuid));
        var emotionsTask = Task.Run(() => _syncDbService.GetUnsyncedEmotions(userGuid, deviceGuid));
        var activitiesTask = Task.Run(() => _syncDbService.GetUnsyncedActivities(userGuid, deviceGuid));
        var emotionCategoriesTask = Task.Run(() => _syncDbService.GetUnsyncedEmotionCategories(userGuid, deviceGuid));

        await Task.WhenAll(journalEntriesTask, emotionsTask, activitiesTask, emotionCategoriesTask);

        var journalEntries = journalEntriesTask.Result;
        var emotions = emotionsTask.Result;
        var activities = activitiesTask.Result;
        var emotionCategories = emotionCategoriesTask.Result;

        var journalEntryRecordSyncs = journalEntries.Select(x => new RecordSync
        {
            DeviceId = deviceGuid,
            RecordId = x.UUID,
            RecordType = RecordType.JournalEntry
        });
        var emotionRecordSyncs = emotions.Select(x => new RecordSync
        {
            DeviceId = deviceGuid,
            RecordId = x.UUID,
            RecordType = RecordType.Emotion
        });
        var activityRecordSyncs = activities.Select(x => new RecordSync
        {
            DeviceId = deviceGuid,
            RecordId = x.UUID,
            RecordType = RecordType.Activity
        });
        var emotionCategoryRecordSyncs = emotionCategories.Select(x => new RecordSync
        {
            DeviceId = deviceGuid,
            RecordId = x.UUID,
            RecordType = RecordType.Category
        });
        var recordSyncs = journalEntryRecordSyncs.Concat(emotionRecordSyncs).Concat(activityRecordSyncs)
            .Concat(emotionCategoryRecordSyncs).ToArray();
        await _syncDbService.MarkSynced(recordSyncs);
        
        return new SyncJournalResponse
        {
            JournalEntries = journalEntries,
            Emotions = emotions,
            Activities = activities,
            EmotionCategories = emotionCategories 
        };
    }
}