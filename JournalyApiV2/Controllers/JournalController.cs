using JournalyApiV2.Models.Requests;
using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Controller]
[Route("/journal")]
public class JournalController : ControllerBase
{
    private readonly IJournalService _journalService;

    public JournalController(IJournalService journalService)
    {
        _journalService = journalService;
    }

    [HttpPatch]
    public async Task<IActionResult> PatchJournal([FromBody] PatchJournalRequest request)
    {
        await _journalService.PatchJournal(request);
        return StatusCode(204);
    }
}