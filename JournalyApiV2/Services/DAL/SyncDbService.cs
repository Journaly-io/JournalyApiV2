
using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using Microsoft.EntityFrameworkCore;
using Medication = JournalyApiV2.Data.Models.Medication;
using MedUnit = JournalyApiV2.Data.Enums.MedUnit;
using RecordType = JournalyApiV2.Data.Enums.RecordType;

namespace JournalyApiV2.Services.DAL;
public class SyncDbService : ISyncDbService
{
    private readonly IDbFactory _db;

    public SyncDbService(IDbFactory db)
    {
        _db = db;
    }

    public async Task MarkSynced(params RecordSync[] recordSyncs)
    {
        await using var db = _db.Journaly();

        var existingRecords =
            await db.SyncedRecords.Where(x => recordSyncs.Select(y => y.RecordId).Contains(x.RecordId) && !x.IsVoid).ToListAsync();
        
        existingRecords.ForEach(x => x.IsVoid = true); // Void existing records

        await db.SyncedRecords.AddRangeAsync(recordSyncs.Select(x => new SyncedRecords
        {
            DeviceId = x.DeviceId,
            RecordId = x.RecordId,
            RecordType = x.RecordType,
            Timestamp = DateTime.UtcNow
        }));
        await db.SaveChangesAsync();
    }

    public async Task<Models.JournalEntry[]> GetUnsyncedJournalEntries(Guid userGuid, Guid deviceGuid)
    {
        await using var db = _db.Journaly();
        var unsyncedJournalEntries =
            from je in db.JournalEntries
            let synced = db.SyncedRecords.Any(sr =>
                sr.RecordId == je.Uuid && sr.DeviceId == deviceGuid && sr.RecordType == RecordType.JournalEntry && !sr.IsVoid)
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

    public async Task<Models.Emotion[]> GetUnsyncedEmotions(Guid userGuid, Guid deviceGuid)
    {
        await using var db = _db.Journaly();
        var unsyncedEmotions =
            from em in db.Emotions
            join iconType in db.IconType on em.IconTypeId equals iconType.Id
            let synced = db.SyncedRecords.Any(sr =>
                sr.RecordId == em.Uuid && sr.DeviceId == deviceGuid && sr.RecordType == RecordType.Emotion && !sr.IsVoid)
            where em.Owner == userGuid && !synced
            select new Models.Emotion
            {
                CategoryId = em.CategoryUuid,
                Icon = em.Icon,
                IconType = iconType.Name,
                Name = em.Name,
                Order = em.Order,
                UUID = em.Uuid,
                Deleted = em.Deleted
            };
        return await unsyncedEmotions.ToArrayAsync();
    }

    public async Task<Models.Activity[]> GetUnsyncedActivities(Guid userGuid, Guid deviceGuid)
    {
        await using var db = _db.Journaly();
        var unsyncedActivities =
            from ac in db.Activities
            join iconType in db.IconType on ac.IconTypeId equals iconType.Id
            let synced = db.SyncedRecords.Any(sr =>
                sr.RecordId == ac.Uuid && sr.DeviceId == deviceGuid && sr.RecordType == RecordType.Activity && !sr.IsVoid)
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

    public async Task<Models.EmotionCategory[]> GetUnsyncedEmotionCategories(Guid userGuid, Guid deviceGuid)
    {
        await using var db = _db.Journaly();
        var unsyncedCategories =
            from ec in db.EmotionCategories
            let synced = db.SyncedRecords.Any(sr =>
                sr.RecordId == ec.Uuid && sr.DeviceId == deviceGuid && sr.RecordType == RecordType.Category && !sr.IsVoid)
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

    public async Task<Models.Medication[]> GetUnsyncedMedications(Guid userGuid, Guid deviceGuid)
    {
        await using var db = _db.Journaly();
        var unsyncedMedications =
            from me in db.Medications
            let synced = db.SyncedRecords.Any(sr =>
                sr.RecordId == me.Uuid && sr.DeviceId == deviceGuid && sr.RecordType == RecordType.Med && !sr.IsVoid)
            where me.Owner == userGuid && !synced
            select new Models.Medication
            {
                DefaultDose = me.DefaultDose,
                Deleted = me.Deleted,
                Forever = me.Forever,
                From = me.FromDate,
                Name = me.Name,
                Notes = me.Notes,
                Schedules = (from schedule in db.MedSchedules
                        where schedule.MedicationUuid == me.Uuid
                            select new Models.Schedule
                            {
                                Time = schedule.Time,
                                EveryOtherDay = schedule.EveryOtherDay,
                                Days = Array.Empty<DayOfWeek>()
                            }).ToArray(),
                Unit = (MedUnit)me.Unit,
                Until = me.UntilDate,
                Uuid = me.Uuid,
                
            };

        return await unsyncedMedications.ToArrayAsync();
    }
}