using JournalyApiV2.Models.Requests;
using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Controller]
[Route("/med")]
[Authorize]
public class MedController : JournalyControllerBase
{
    private readonly IMedService _medService;

    public MedController(IMedService medService)
    {
        _medService = medService;
    }

    [HttpPatch]
    public async Task<IActionResult> PatchMeds([FromBody] PatchMedsRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(errors);
        }
        await _medService.PatchMeds(request, GetUserId(), GetDeviceId());
        return StatusCode(204);
    }
}