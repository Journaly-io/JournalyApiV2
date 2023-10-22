using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Controller]
[Route("/sync")]
public class SyncController : JournalyControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    [Route("journal")]
    [HttpGet]
    public async Task<JsonResult> SyncJournal()
    {
        return new JsonResult(await _syncService.GetUnsyncedJournalData(GetUserId(), GetDeviceId()));
    }

    [Route("med")]
    [HttpGet]
    public async Task<JsonResult> SyncMeds()
    {
        return new JsonResult(await _syncService.GetUnsyncedMedData(GetUserId(), GetDeviceId()));
    }
}