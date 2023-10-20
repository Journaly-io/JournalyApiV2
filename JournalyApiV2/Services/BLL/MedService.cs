using JournalyApiV2.Models.Requests;
using JournalyApiV2.Services.DAL;

namespace JournalyApiV2.Services.BLL;

public class MedService : IMedService
{
    private readonly IMedDbService _medDbService;

    public MedService(IMedDbService medDbService)
    {
        _medDbService = medDbService;
    }

    public async Task PatchMeds(PatchMedsRequest request, Guid owner, Guid deviceId)
    {
        await _medDbService.SyncMeds(request.Meds, owner, deviceId);
    }
}