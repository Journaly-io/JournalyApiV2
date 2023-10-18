using JournalyApiV2.Models.Requests;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Controller]
[Route("/journal")]
public class JournalController : JournalyControllerBase
{
    private readonly IJournalService _journalService;
    private readonly IResourceAccessHelper _resourceAccessHelper;

    public JournalController(IJournalService journalService, IResourceAccessHelper resourceAccessHelper)
    {
        _journalService = journalService;
        _resourceAccessHelper = resourceAccessHelper;
    }

    [HttpPatch]
    public async Task<IActionResult> PatchJournal([FromBody] PatchJournalRequest request)
    {
        try
        {
            await Task.WhenAll(
                Task.Run(() => _resourceAccessHelper.ValidateCategoryAccess(GetUserId(), request.Categories.Select(x => x.Uuid).ToArray()))
            );
        }
        catch (ResourceAccessHelper.NoAccessException)
        {
            return StatusCode(403);
        }
        await _journalService.PatchJournal(request, GetUserId());
        return StatusCode(204);
    }
}