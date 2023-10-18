using JournalyApiV2.Models;
using JournalyApiV2.Models.Requests;

namespace JournalyApiV2.Services.DAL;

public interface IJournalDbService
{
    Task SyncCategories(PatchJournalRequest.CategoryPatch[] categories);
}