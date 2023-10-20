using JournalyApiV2.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Controller]
[Route("/med")]
[Authorize]
public class MedController : JournalyControllerBase
{
    [HttpPatch]
    public async Task<IActionResult> PatchMeds([FromBody] PatchMedsRequest request)
    {
        return StatusCode(204);
    }
}