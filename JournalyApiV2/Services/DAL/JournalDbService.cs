using JournalyApiV2.Data;
using JournalyApiV2.Models;

namespace JournalyApiV2.Services.DAL;

public class JournalDbService
{
    private readonly JournalyDbContext _db;

    public JournalDbService(JournalyDbContext db)
    {
        _db = db;
    }

    public async Task<EmotionCategory> GetCategoryByUuid(Guid uuid)
    {
        var result = await _db.EmotionCategories.FindAsync(uuid);
        if (result == null) throw new KeyNotFoundException("Emotion Category not found");
        return new EmotionCategory
        {
            AllowMultiple = result.AllowMultiple,
            Default = result.Default,
            Deleted = result.Deleted,
            Name = result.Name,
            Order = result.Order,
            UUID = result.Uuid
        };
    }
    
    public async Task SyncCategories(EmotionCategory[] categories)
    {
        
    }
}