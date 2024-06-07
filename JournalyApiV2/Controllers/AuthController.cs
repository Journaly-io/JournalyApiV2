using System.Security.Claims;
using JournalyApiV2.Data.Enums;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Requests;
using JournalyApiV2.Models.Responses;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SendGrid.Helpers.Errors.Model;

namespace JournalyApiV2.Controllers;

[Controller]
[Route("/auth")]
public class AuthController : JournalyControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICryptoService _cryptoService;

    public AuthController(IAuthService authService, ICryptoService cryptoService)
    {
        _authService = authService;
        _cryptoService = cryptoService;
    }

    [Route("new-user")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] NewUserRequest request)
    {
        try
        {
            await _authService.CreateUser(request.Email, request.Password, request.FirstName, request.LastName,
                request.EncryptedDEK, request.KEKSalt);
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
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            await _authService.ChangePassword(GetUserId(), request.OldPassword, request.NewPassword,
                request.encryptedDEK, request.KEKSalt,
                Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""),
                request.SignOutEverywhere);
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
        await _authService.SignOutEverywhereAsync(GetUserId(),
            Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""));
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

    [Route("upload-key")]
    [HttpPost]
    public async Task<IActionResult> UploadKey([FromBody] CryptographicKey request)
    {
        await _cryptoService.StoreNewDEKForUser(GetUserId(), request.DEK, request.Salt, (EncryptedDEKType)request.Type);
        return StatusCode(204);
    }

    [Route("begin-account-recovery")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> BeginAccountRecovery([FromBody] BeginAccountRecoveryRequest request)
    {
        await _authService.BeginAccountRecovery(request.Email);
        return StatusCode(204); // We will pretend this worked even if the email is not found
    }

    [Route("verify-email-for-account-recovery")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<JsonResult> VerifyEmailForAccountRecovery([FromBody] VerifyEmailRequest request)
    {
        if (!string.IsNullOrEmpty(request.LongCode))
            return new JsonResult(new AuthenticationResponse
            {
                Token = await _authService.IssueRecoveryTokenWithLongCode(request.LongCode)
            });
        if (string.IsNullOrEmpty(request.ShortCode)) throw new HttpBadRequestException("No recovery code provided");
        if (string.IsNullOrEmpty(request.Email))
            throw new HttpBadRequestException("Short code provided but no email provided where it is required");
        return new JsonResult(new AuthenticationResponse
        {
            Token = await _authService.IssueRecoveryTokenWithShortCode(request.Email, request.ShortCode)
        });
    }

    [Route("get-recovery-keys")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<JsonResult> GetRecoveryKeys()
    {
        return new JsonResult(await _authService.GetRecoveryKeys(GetRecoveryToken()));
    }
}