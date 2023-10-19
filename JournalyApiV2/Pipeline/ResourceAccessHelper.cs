using JournalyApiV2.Data;

namespace JournalyApiV2.Pipeline;

public class ResourceAccessHelper : IResourceAccessHelper
{
    private readonly IDbFactory _db;

    public ResourceAccessHelper(IDbFactory dbFactory)
    {
        _db = dbFactory;
    }

    public async Task ValidateCategoryAccess(Guid userId, params Guid[] emotionCategories)
    {
        await Task.WhenAll(emotionCategories.Select(x => Task.Run(() => ValidateSingleCategory(x, userId))));
    }

    private async Task ValidateSingleCategory(Guid category, Guid userId)
    {
        await using var db = _db.Journaly();
        var dbCategory = await db.EmotionCategories.FindAsync(category);
        if (dbCategory == null) return; // This is a new category, so the user inherently has access
        if (dbCategory.Owner == userId) return;
        throw new IResourceAccessHelper.NoAccessException();
    }

    public async Task ValidateEmotionAccess(Guid userId, params Guid[] emotions)
    {
        await Task.WhenAll(emotions.Select(x => Task.Run(() => ValidateSingleEmotion(x, userId))));
    }

    private async Task ValidateSingleEmotion(Guid emotion, Guid userId)
    {
        await using var db = _db.Journaly();
        var dbEmotion = await db.Emotions.FindAsync(emotion);
        if (dbEmotion == null) return; // This is a new emotion, so the user inherently has access
        if (dbEmotion.Owner == userId) return;
        throw new IResourceAccessHelper.NoAccessException();
    }

    public async Task ValidateActivityAccess(Guid userId, params Guid[] activities)
    {
        await Task.WhenAll(activities.Select(x => Task.Run(() => ValidateSingleActivity(x, userId))));
    }

    private async Task ValidateSingleActivity(Guid activity, Guid userId)
    {
        await using var db = _db.Journaly();
        var dbActivity = await db.Activities.FindAsync(activity);
        if (dbActivity == null) return; // This is a new activity, so the user inherently has access
        if (dbActivity.Owner == userId) return;
        throw new IResourceAccessHelper.NoAccessException();
    }

    public async Task ValidateJournalEntryAccess(Guid userId, params Guid[] journalEntries)
    {
        await Task.WhenAll(journalEntries.Select(x => Task.Run(() => ValidateSingleJournalEntry(x, userId))));
    }

    private async Task ValidateSingleJournalEntry(Guid entry, Guid userId)
    {
        await using var db = _db.Journaly();
        var dbJournalEntry = await db.JournalEntries.FindAsync(entry);
        if (dbJournalEntry == null) return; // This is a new entry, so the user inherently has access
        if (dbJournalEntry.Owner == userId) return;
        throw new IResourceAccessHelper.NoAccessException();
    }

}