using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Controller]
public class AccountResetController : JournalyControllerBase
{
    private readonly IMedService _medService;

    public AccountResetController(IMedService medService)
    {
        _medService = medService;
    }

    // This endpoint is for the local backup restore functionality. It clears the account data without deleting the account
    [HttpGet]
    [Route("/account-reset")]
    public async Task<IActionResult> AccountReset()
    {
        await _medService.ClearMeds(GetUserId());

        return StatusCode(204);
    }
}