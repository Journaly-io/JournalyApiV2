using JournalyApiV2.Data;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Requests;

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
        var tasks = categories.Select(emotionCategory => Task.Run(() => SyncSingleCategory(emotionCategory, owner))).ToArray();
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
        
    }
}