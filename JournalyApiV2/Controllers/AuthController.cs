using System.Security.Claims;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Requests;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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
            await _authService.CreateUser(request.Email, request.Password, request.FirstName, request.LastName, request.EncryptedDEK, request.KEKSalt);
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
            var token = User.FindFirst("token");
            if (token != null) await _authService.VoidToken(token.Value);
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
        await _authService.ChangeName(request.FirstName, request.LastName, GetUserId());
        return StatusCode(204);
    }

    [Route("change-email")]
    [HttpPost]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
    {
        await _authService.ChangeEmail(request.Email, request.PasswordHash, GetUserId());
        return StatusCode(204);
    }

    [Route("change-password")]
    // This is to bypass the email-confirmed policy. This is in case the user makes an account and signs out before they verify their email and forget their password
    [AllowAnonymous]
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            await _authService.ChangePassword(GetUserId(), request.OldPassword, request.NewPassword, request.encryptedDEK, request.KEKSalt, Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""), request.SignOutEverywhere);
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

    [Route("sign-out-everywhere")]
    [HttpGet]
    public async Task<IActionResult> SignOutEverywhere()
    {
        await _authService.SignOutEverywhereAsync(GetUserId(), Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""));
        return StatusCode(204);
    }

    [Route("userinfo")]
    // This is to bypass the email-confirmed policy
    [AllowAnonymous]
    [Authorize]
    [HttpGet]
    public async Task<JsonResult> UserInfo()
    {
        var result = await _authService.GetUserInfoAsync(GetUserId());
        return new JsonResult(result);
    }
}