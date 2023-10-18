using JournalyApiV2.Data;

namespace JournalyApiV2.Pipeline;

public class ResourceAccessHelper : IResourceAccessHelper
{
    private readonly JournalyDbContext _db;

    public ResourceAccessHelper(JournalyDbContext db)
    {
        _db = db;
    }

    public async Task ValidateCategoryAccess(Guid userId, params Guid[] emotionCategories)
    {
        await Task.WhenAll(emotionCategories.Select(x => Task.Run(() => ValidateSingleCategory(x, userId))));
    }

    private async Task ValidateSingleCategory(Guid category, Guid userId)
    {
        var dbCategory = await _db.EmotionCategories.FindAsync(category);
        if (dbCategory == null) return; // This is a new category, so the user inherently has access
        if (dbCategory.Owner == userId) return;
        throw new NoAccessException();
    }

    public class NoAccessException : Exception
    {
        
    }
}