using System.Security.Claims;
using JournalyApiV2.Models.Requests;
using JournalyApiV2.Pipeline;
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
        catch (ArgumentException ex)
        {
            return StatusCode(409, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

        return StatusCode(204);
    }

    [Route("sign-in")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<JsonResult> SignIn([FromBody] SignInRequest request)
    {
        try
        {
            var result = await _authService.SignIn(request.Email, request.Password);
            return new JsonResult(result);
        }
        catch (ArgumentException ex)
        {
            throw new HttpBadRequestException(ex.Message);
        }
    }

    [Route("refresh-token")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<JsonResult> RefreshToken([FromBody] string token)
    {
        try
        {
            var result = await _authService.RefreshToken(token);
            return new JsonResult(result);
        }
        catch (ArgumentException ex)
        {
            throw new HttpBadRequestException(ex.Message);
        }
    }

    [Route("change-name")]
    [HttpPost]
    public async Task<JsonResult> ChangeName([FromBody] ChangeNameRequest request)
    {
        var tokenId = User.FindFirst("token_id");
        if (tokenId == null) throw new HttpBadRequestException("Token has no identifier");
        var result =
            await _authService.ChangeName(request.FirstName, request.LastName, GetUserId(), int.Parse(tokenId.Value));
        return new JsonResult(result);
    }

    [Route("change-email")]
    [HttpPost]
    public async Task<JsonResult> ChangeEmail([FromBody] ChangeEmailRequest request)
    {
        var tokenId = User.FindFirst("token_id");
        if (tokenId == null) throw new HttpBadRequestException("Token has no identifier");
        var result =
            await _authService.ChangeEmail(request.Email, GetUserId(), int.Parse(tokenId.Value));
        return new JsonResult(result);
    }

    [Route("change-password")]
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var tokenId = User.FindFirst("token_id");
        if (tokenId == null) throw new HttpBadRequestException("Token has no identifier");
        try
        {
            await _authService.ChangePassword(GetUserId(), request.OldPassword, request.NewPassword,
                int.Parse(tokenId.Value));
        }
        catch (ArgumentException)
        {
            throw new HttpBadRequestException("Incorrect current password");
        }

        return StatusCode(204);
    }
}