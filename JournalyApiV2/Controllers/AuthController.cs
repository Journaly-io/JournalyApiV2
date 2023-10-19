using JournalyApiV2.Models.Requests;
using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Controller]
[Route("/auth")]
public class AuthController : JournalyControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [Route("new-user")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] NewUserRequest request)
    {
        try
        {
            await _authService.CreateUser(request.Email, request.Password, request.FirstName, request.LastName);
        }
        catch (Exception ex)
        {
            if (ex.Message == "Conflict") return StatusCode(409);
            throw;
        }
        return StatusCode(204);
    }
}