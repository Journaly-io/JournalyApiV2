namespace JournalyApiV2.Pipeline;

public interface IResourceAccessHelper
{
    Task ValidateCategoryAccess(Guid userId, params Guid[] emotionCategories);
}