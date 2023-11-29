using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Responses;
using JournalyApiV2.Services.DAL;
using Activity = JournalyApiV2.Models.Activity;
using Emotion = JournalyApiV2.Models.Emotion;
using EmotionCategory = JournalyApiV2.Models.EmotionCategory;
using JournalEntry = JournalyApiV2.Models.JournalEntry;
using Medication = JournalyApiV2.Models.Medication;
using RecordType = JournalyApiV2.Data.Enums.RecordType;

namespace JournalyApiV2.Services.BLL;

public class SyncService : ISyncService
{
    private readonly ISyncDbService _syncDbService;

    public SyncService(ISyncDbService syncDbService)
    {
        _syncDbService = syncDbService;
    }

    public async Task<SyncMedResponse> GetUnsyncedMedData(Guid userGuid, Guid deviceGuid, int size)
    {
        var medsTask = Task.Run(() => _syncDbService.GetUnsyncedMedications(userGuid, deviceGuid));
        var schedulesTask = Task.Run(() => _syncDbService.GetUnsyncedSchedules(userGuid, deviceGuid));
        var instancesTask = Task.Run(() => _syncDbService.GetUnsyncedMedInstances(userGuid, deviceGuid));

        await Task.WhenAll(medsTask, schedulesTask, instancesTask);

        Medication[] meds;
        Schedule[] schedules;
        MedInstance[] instances;
        
        if (size == 0)
        {
            meds = medsTask.Result;
            schedules = schedulesTask.Result;
            instances = instancesTask.Result;
        }
        else
        {
            var count = size;
            
            meds = medsTask.Result.Take(count).ToArray();
            count -= meds.Length;

            schedules = schedulesTask.Result.Take(count).ToArray();
            count -= schedules.Length;

            instances = instancesTask.Result.Take(count).ToArray();
        }

        var medSyncs = meds.Select(x => new RecordSync
        {
            DeviceId = deviceGuid,
            RecordId = x.Uuid,
            RecordType = RecordType.Med
        });

        var scheduleSyncs = schedules.Select(x => new RecordSync
        {
            DeviceId = deviceGuid,
            RecordId = x.Uuid,
            RecordType = RecordType.Schedule
        });

        var instanceSyncs = instances.Select(x => new RecordSync
        {
            DeviceId = deviceGuid,
            RecordId = x.Uuid,
            RecordType = RecordType.MedInstance
        });

        var recordSyncs = medSyncs.Concat(scheduleSyncs).Concat(instanceSyncs).ToArray();

        var totalRecords = medsTask.Result.Length + schedulesTask.Result.Length + instancesTask.Result.Length;
        
        await _syncDbService.MarkSynced(recordSyncs);

        return new SyncMedResponse
        {
            Medications = meds,
            Schedules = schedules,
            MedInstances = instances,
            Remaining = totalRecords - (meds.Length + schedules.Length + instances.Length)
        };
    }
    
    public async Task<SyncJournalResponse> GetUnsyncedJournalData(Guid userGuid, Guid deviceGuid, int size)
    {
        var emotionCategoriesTask = Task.Run(() => _syncDbService.GetUnsyncedEmotionCategories(userGuid, deviceGuid));
        var journalEntriesTask = Task.Run(() => _syncDbService.GetUnsyncedJournalEntries(userGuid, deviceGuid));
        var emotionsTask = Task.Run(() => _syncDbService.GetUnsyncedEmotions(userGuid, deviceGuid));
        var activitiesTask = Task.Run(() => _syncDbService.GetUnsyncedActivities(userGuid, deviceGuid));

        await Task.WhenAll(journalEntriesTask, emotionsTask, activitiesTask, emotionCategoriesTask);

        EmotionCategory[] emotionCategories;
        Emotion[] emotions;
        Activity[] activities;
        JournalEntry[] journalEntries;
        if (size == 0)
        {
            emotionCategories = emotionCategoriesTask.Result;
            emotions = emotionsTask.Result;
            activities = activitiesTask.Result;
            journalEntries = journalEntriesTask.Result;
        }
        else
        {
            var count = size;
           
            emotionCategories = emotionCategoriesTask.Result.Take(count).ToArray();
            count -= emotionCategories.Length;
            
            emotions = emotionsTask.Result.Take(count).ToArray();
            count -= emotions.Length;
            
            activities = activitiesTask.Result.Take(size).ToArray();
            count -= activities.Length;
            
            journalEntries = journalEntriesTask.Result.Take(count).ToArray();
        }


        var totalRecords = journalEntriesTask.Result.Length + emotionsTask.Result.Length +
                           activitiesTask.Result.Length + emotionCategoriesTask.Result.Length;

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
            EmotionCategories = emotionCategories,
            Remaining = totalRecords - (journalEntries.Length + emotions.Length + activities.Length + emotionCategories.Length)
        };
    }
}