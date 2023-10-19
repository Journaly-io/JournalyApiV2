using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace JournalyApiV2.Services.DAL;

public class JournalDbService : IJournalDbService
{
    private readonly JournalyDbContext _db;

    public JournalDbService(JournalyDbContext db)
    {
        _db = db;
    }

    // Categories
    public async Task SyncCategories(PatchJournalRequest.CategoryPatch[] categories, Guid owner)
    {
        var tasks = categories.Select(emotionCategory => Task.Run(() => SyncSingleCategory(emotionCategory, owner)))
            .ToArray();
        await Task.WhenAll(tasks);
    }

    private async Task SyncSingleCategory(PatchJournalRequest.CategoryPatch emotionCategory, Guid owner)
    {
        var category = await _db.EmotionCategories.FindAsync(emotionCategory.Uuid);
        if (category == null) // New category
        {
            category = new Data.Models.EmotionCategory
            {
                Uuid = emotionCategory.Uuid,
                Owner = owner
            };
            await _db.EmotionCategories.AddAsync(category);
        }

        if (emotionCategory.AllowMultiple != null) category.AllowMultiple = emotionCategory.AllowMultiple.Value;
        if (emotionCategory.Default != null) category.Default = emotionCategory.Default.Value;
        if (emotionCategory.Deleted != null) category.Deleted = emotionCategory.Deleted.Value;
        if (emotionCategory.Name != null) category.Name = emotionCategory.Name;
        if (emotionCategory.Order != null) category.Order = emotionCategory.Order.Value;

        await _db.SaveChangesAsync();
    }

    // Emotions
    public async Task SyncEmotions(PatchJournalRequest.EmotionPatch[] emotions, Guid owner)
    {
        var tasks = emotions.Select(emotion => Task.Run(() => SyncSingleEmotion(emotion, owner)));
        await Task.WhenAll(tasks);
    }

    private async Task SyncSingleEmotion(PatchJournalRequest.EmotionPatch emotion, Guid owner)
    {
        var dbEmotion = await _db.Emotions.FindAsync(emotion.Uuid);
        if (dbEmotion == null)
        {
            if (emotion.Category == null)
            {
                throw new ArgumentException("New emotions must include a category uuid");
            }

            dbEmotion = new Data.Models.Emotion
            {
                Uuid = emotion.Uuid,
                Owner = owner,
                CategoryUuid = emotion.Category.Value
            };
            await _db.Emotions.AddAsync(dbEmotion);
        }

        if (emotion.Deleted != null) dbEmotion.Deleted = emotion.Deleted.Value;
        if (emotion.Name != null) dbEmotion.Name = emotion.Name;
        if (emotion.Order != null) dbEmotion.Order = emotion.Order.Value;
        if (emotion.Icon != null) dbEmotion.Icon = emotion.Icon;
        if (emotion.IconType != null) dbEmotion.IconTypeId = await GetIconTypeIdByName(emotion.IconType);

        await _db.SaveChangesAsync();
    }

    // Activities
    public async Task SyncActivities(PatchJournalRequest.ActivityPatch[] activities, Guid owner)
    {
        var tasks = activities.Select(activity => Task.Run(() => SyncSingleActivity(activity, owner)));
        await Task.WhenAll(tasks);
    }

    private async Task SyncSingleActivity(PatchJournalRequest.ActivityPatch activity, Guid owner)
    {
        var dbActivity = await _db.Activities.FindAsync(activity.Uuid);
        if (dbActivity == null)
        {
            dbActivity = new Data.Models.Activity
            {
                Uuid = activity.Uuid,
                Owner = owner
            };
            await _db.Activities.AddAsync(dbActivity);
        }

        if (activity.Deleted != null) dbActivity.Deleted = activity.Deleted.Value;
        if (activity.IconType != null) dbActivity.IconTypeId = await GetIconTypeIdByName(activity.IconType);
        if (activity.Name != null) dbActivity.Name = activity.Name;
        if (activity.Order != null) dbActivity.Order = activity.Order.Value;
        if (activity.Icon != null) dbActivity.Icon = activity.Icon;

        await _db.SaveChangesAsync();
    }

    public async Task SyncJournalEntries(PatchJournalRequest.JournalPatch[] entries, Guid owner)
    {
        await Task.WhenAll(entries.Select(entry => Task.Run(() => SyncSingleJournalEntry(entry, owner))));
    }

    private async Task SyncSingleJournalEntry(PatchJournalRequest.JournalPatch entry, Guid owner)
    {
        var dbJournalEntry = await _db.JournalEntries.Include(x => x.ActivityEntries)
            .Include(x => x.JournalEntryCategoryValues).Include(x => x.EmotionEntries)
            .SingleOrDefaultAsync(x => x.Uuid == entry.Uuid);
        if (dbJournalEntry == null)
        {
            dbJournalEntry = new Data.Models.JournalEntry
            {
                Uuid = entry.Uuid,
                Owner = owner,
                ActivityEntries = new List<ActivityEntry>(),
                EmotionEntries = new List<EmotionEntry>(),
                JournalEntryCategoryValues = new List<JournalEntryCategoryValue>()
            };
            await _db.JournalEntries.AddAsync(dbJournalEntry);
        }

        // Handle the easy stuff first - the direct properties of the journal entry
        dbJournalEntry.Body = entry.Body;
        dbJournalEntry.Deleted = entry.Deleted;
        dbJournalEntry.CreatedAt = entry.CreatedAt;
        
        // Handle emotions
        // Add any missing emotions
        foreach (var emotionEntry in entry.Emotions)
        {
            if (!dbJournalEntry.EmotionEntries.Select(x => x.EmotionUuid).Contains(emotionEntry))
            {
                dbJournalEntry.EmotionEntries.Add(new EmotionEntry
                {
                    EmotionUuid = emotionEntry,
                    JournalEntryUuid = dbJournalEntry.Uuid
                });
            }
        }

        // Remove any removed emotions
        var emotionsToRemove = new List<EmotionEntry>();
        foreach (var emotionEntry in dbJournalEntry.EmotionEntries)
        {
            if (!entry.Emotions.Contains(emotionEntry.EmotionUuid))
            {
                emotionsToRemove.Add(emotionEntry);
            }
        }

        foreach (var emotionEntry in emotionsToRemove)
        {
            dbJournalEntry.EmotionEntries.Remove(emotionEntry);
        }

        // Handle activities
        // Add any missing activities
        foreach (var activityEntry in entry.Activities)
        {
            if (!dbJournalEntry.ActivityEntries.Select(x => x.ActivityUuid).Contains(activityEntry))
            {
                dbJournalEntry.ActivityEntries.Add(new ActivityEntry
                {
                    ActivityUuid = activityEntry,
                    JournalEntryUuid = dbJournalEntry.Uuid
                });
            }
        }

        // Remove any removed activities
        var activitiesToRemove = new List<ActivityEntry>();
        foreach (var activityEntry in dbJournalEntry.ActivityEntries)
        {
            if (!entry.Activities.Contains(activityEntry.ActivityUuid))
            {
                activitiesToRemove.Add(activityEntry);
            }
        }

        foreach (var activityEntry in activitiesToRemove)
        {
            dbJournalEntry.ActivityEntries.Remove(activityEntry);
        }
        
        // Finally, the hard part. Handle category values
        // Handle new/updated entries
        foreach (var categoryValue in entry.CategoryValues)  
        {
            // Check if it already exists
            var existing =
                dbJournalEntry.JournalEntryCategoryValues.SingleOrDefault(x => x.CategoryUuid == categoryValue.Uuid);
            if (existing != null) // It does, make sure its up to date
            {
                existing.Value = categoryValue.Value;
            }
            else // It does not - create it
            {
                dbJournalEntry.JournalEntryCategoryValues.Add(new JournalEntryCategoryValue
                {
                    CategoryUuid = categoryValue.Uuid,
                    JournalEntryUuid = dbJournalEntry.Uuid,
                    Value = categoryValue.Value
                });
            }
        }
        // Handle removed entries
        var categoryValuesToRemove = new List<JournalEntryCategoryValue>();
        foreach (var journalEntryCategoryValue in dbJournalEntry.JournalEntryCategoryValues)
        {
            if (!entry.CategoryValues.Select(x => x.Uuid).Contains(journalEntryCategoryValue.CategoryUuid))
            {
                categoryValuesToRemove.Add(journalEntryCategoryValue);
            }
        }

        foreach (var categoryValue in categoryValuesToRemove)
        {
            dbJournalEntry.JournalEntryCategoryValues.Remove(categoryValue);
        }
        
        // Finally, save everything
        await _db.SaveChangesAsync();
    }


    private async Task<short> GetIconTypeIdByName(string name)
    {
        return (await _db.IconType.SingleAsync(x => x.Name == name)).Id;
    }
}