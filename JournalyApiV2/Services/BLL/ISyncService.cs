using JournalyApiV2.Models.Responses;

namespace JournalyApiV2.Services.BLL;

public interface ISyncService
{
    Task<SyncJournalResponse> GetUnsyncedJournalData(Guid userGuid, Guid deviceGuid, int size);
    Task<SyncMedResponse> GetUnsyncedMedData(Guid userGuid, Guid deviceGuid, int size);
}