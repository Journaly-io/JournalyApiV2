namespace JournalyApiV2.Pipeline;

public interface IResourceAccessHelper
{
    Task ValidateCategoryAccess(Guid userId, params Guid[] emotionCategories);
    Task ValidateEmotionAccess(Guid userId, params Guid[] emotions);

    class NoAccessException : Exception
    {
        
    }

    Task ValidateActivityAccess(Guid userId, params Guid[] activities);
    Task ValidateJournalEntryAccess(Guid userId, params Guid[] journalEntries);
}