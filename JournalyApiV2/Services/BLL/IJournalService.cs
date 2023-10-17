using JournalyApiV2.Models.Requests;

namespace JournalyApiV2.Services.BLL;

public interface IJournalService
{
    Task PatchJournal(PatchJournalRequest request);
}