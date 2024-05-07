using System.Security.Claims;
using JournalyApiV2.Models;
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
    

    [Route("sign-out")]
    [HttpGet]
    public async Task<ActionResult> SignOut()
    {
        try
        {
            var tokenId = User.FindFirst("token_id");
            if (tokenId != null) await _authService.VoidToken(Convert.ToInt32(tokenId.Value));
        }
        catch (ArgumentException)
        {
            // ignored
        }

        return Ok();
    }

    [Route("change-name")]
    [HttpPost]
    public async Task<IActionResult> ChangeName([FromBody] ChangeNameRequest request)
    {
        var tokenId = User.FindFirst("token_id");
        if (tokenId == null) throw new HttpBadRequestException("Token has no identifier");
        return StatusCode(204);
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
                int.Parse(tokenId.Value), request.SignOutEverywhere);
        }
        catch (ArgumentException)
        {
            throw new HttpBadRequestException("Incorrect current password");
        }

        return StatusCode(204);
    }

    [Route("verify-email")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        if (request.LongCode != null)
        {
            try
            {
                await _authService.VerifyEmailWithLongCode(request.LongCode);
            }
            catch (ArgumentException ex)
            {
                throw new HttpBadRequestException(ex.Message);
            }
        }
        else
        {
            if (request.ShortCode == null) throw new HttpBadRequestException("No verification code provided");
            try
            {
                await _authService.VerifyEmailWithShortCode(GetUserId(), request.ShortCode);
            }
            catch (ArgumentException ex)
            {
                throw new HttpBadRequestException(ex.Message);
            }
        }

        return StatusCode(204);
    }

    [Route("poll-email-verification")]
    [HttpGet]
    public async Task<IActionResult> PollEmailVerification()
    {
        return StatusCode(204); // The middleware will force a 403 if email is not verified
    }

    [Route("resend-verification-email")]
    [HttpGet]
    // This is to bypass the email-confirmed policy
    [AllowAnonymous]
    [Authorize]
    public async Task<IActionResult> ResendVerificationEmail()
    {
        try
        {
            await _authService.ResendVerificationEmailAsync(GetUserId());
        }
        catch (TooEarlyException ex)
        {
            throw new HttpBadRequestException(ex.Message);
        }
        catch (ArgumentException ex)
        {
            throw new HttpBadRequestException(ex.Message);
        }

        return StatusCode(204);
    }

    [Route("reset-password")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] string email)
    {
        try
        {
            await _authService.ResetPasswordAsync(email);
        }
        catch (ArgumentException)
        {
            // Ignore argumentException - this means the email was not found but we don't want the user to know that
        }
        catch (TooEarlyException ex)
        {
            throw new HttpBadRequestException(ex.Message);
        }
        return StatusCode(204);
    }
    
    [Route("submit-password-reset")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitPasswordReset([FromBody] ResetPasswordRequest request)
    {
        try
        {
            await _authService.SubmitPasswordResetAsync(request.Code, request.NewPassword, request.SignOutEverywhere);
        }
        catch (ArgumentException ex)
        {
            throw new HttpBadRequestException(ex.Message);
        }
        return StatusCode(204);
    }

    [Route("sign-out-everywhere")]
    [HttpGet]
    public async Task<IActionResult> SignOutEverywhere()
    {
        var tokenId = User.FindFirst("token_id");
        if (tokenId == null) throw new HttpBadRequestException("Token has no identifier");
        await _authService.SignOutEverywhereAsync(GetUserId(), Convert.ToInt32(tokenId.Value));

        return StatusCode(204);
    }
    
}