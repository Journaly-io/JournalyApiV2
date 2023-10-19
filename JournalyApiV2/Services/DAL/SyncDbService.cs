
using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using Microsoft.EntityFrameworkCore;
using RecordType = JournalyApiV2.Data.Enums.RecordType;

namespace JournalyApiV2.Services.DAL;
public class SyncDbService : ISyncDbService
{
    private readonly IDbFactory _db;

    public async Task MarkSynced(params RecordSync[] recordSyncs)
    {
        using (var db = _db.Journaly())
        {
            await db.SyncedRecords.AddRangeAsync(recordSyncs.Select(x => new SyncedRecords
            {
                DeviceId = x.DeviceId,
                RecordId = x.RecordId,
                RecordTypeId = (short)x.RecordType,
                Timestamp = DateTime.UtcNow
            }));
            await db.SaveChangesAsync();
        }
    }

    public async Task<Models.JournalEntry[]> GetUnsyncedJournalEntries(Guid userGuid, Guid deviceGuid)
    {
        using (var db = _db.Journaly())
        {
            var unsyncedJournalEntries =
                from je in db.JournalEntries
                let synced = db.SyncedRecords.Any(sr =>
                    sr.RecordId == je.Uuid && sr.DeviceId == deviceGuid && sr.RecordTypeId == (int)RecordType.JournalEntry)
                where je.Owner == userGuid && !synced
                select new Models.JournalEntry
                {
                    UUID = je.Uuid,
                    Text = je.Body,
                    CreatedAt = je.CreatedAt,
                    Activities = (from activityEntry in db.ActivityEntries
                        join activity in db.Activities on activityEntry.ActivityUuid equals activity.Uuid
                        where activityEntry.JournalEntryUuid == je.Uuid
                        select new Models.Activity
                        {
                            UUID = activity.Uuid,
                            Name = activity.Name
                        }).ToList(),
                    Emotions = (from emotionEntry in db.EmotionEntries
                        join emotion in db.Emotions on emotionEntry.EmotionUuid equals emotion.Uuid
                        where emotionEntry.JournalEntryUuid == je.Uuid
                        select new Models.Emotion
                        {
                            UUID = emotion.Uuid,
                            Name = emotion.Name
                        }).ToList(),
                    CategoryValues = (from categoryValue in db.JournalEntryCategoryValues
                        where categoryValue.JournalEntryUuid == je.Uuid
                        select new Models.CategoryValue
                        {
                            CategoryUuid = categoryValue.CategoryUuid,
                            JournalEntryUuid = categoryValue.JournalEntryUuid,
                            Value = categoryValue.Value
                        }).ToList()
                };

            return await unsyncedJournalEntries.ToArrayAsync();
        }
    }

    public async Task<Models.Emotion[]> GetUnsyncedEmotions(Guid userGuid, Guid deviceGuid)
    {
        using (var db = _db.Journaly())
        {
            var unsyncedEmotions =
                from em in db.Emotions
                join iconType in db.IconType on em.IconTypeId equals iconType.Id
                let synced = db.SyncedRecords.Any(sr =>
                    sr.RecordId == em.Uuid && sr.DeviceId == deviceGuid && sr.RecordTypeId == (int)RecordType.Emotion)
                where em.Owner == userGuid && !synced
                select new Models.Emotion
                {
                    CategoryId = em.CategoryUuid,
                    Icon = em.Icon,
                    IconType = iconType.Name,
                    Name = em.Name,
                    Order = em.Order,
                    UUID = em.Uuid
                };
            return await unsyncedEmotions.ToArrayAsync();
        }
    }

    public async Task<Models.Activity[]> GetUnsyncedActivities(Guid userGuid, Guid deviceGuid)
    {
        using (var db = _db.Journaly())
        {
            var unsyncedActivities =
                from ac in db.Activities
                join iconType in db.IconType on ac.IconTypeId equals iconType.Id
                let synced = db.SyncedRecords.Any(sr =>
                    sr.RecordId == ac.Uuid && sr.DeviceId == deviceGuid && sr.RecordTypeId == (int)RecordType.Activity)
                where ac.Owner == userGuid && !synced
                select new Models.Activity
                {
                    Icon = ac.Icon,
                    IconType = iconType.Name,
                    Name = ac.Name,
                    Order = ac.Order,
                    UUID = ac.Uuid
                };

            return await unsyncedActivities.ToArrayAsync();
        }
    }

    public async Task<Models.EmotionCategory[]> GetUnsyncedEmotionCategories(Guid userGuid, Guid deviceGuid)
    {
        using (var db = _db.Journaly())
        {
            var unsyncedCategories =
                from ec in db.EmotionCategories
                let synced = db.SyncedRecords.Any(sr =>
                    sr.RecordId == ec.Uuid && sr.DeviceId == deviceGuid && sr.RecordTypeId == (int)RecordType.Category)
                where ec.Owner == userGuid && !synced
                select new Models.EmotionCategory
                {
                    AllowMultiple = ec.AllowMultiple,
                    Default = ec.Default,
                    Name = ec.Name,
                    Order = ec.Order,
                    UUID = ec.Uuid,
                    Deleted = ec.Deleted
                };

            return await unsyncedCategories.ToArrayAsync();
        }
    }
}