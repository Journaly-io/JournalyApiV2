using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using JournalyApiV2.Data;
using JournalyApiV2.Data.Models;
using JournalyApiV2.Models;
using JournalyApiV2.Models.Responses;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.DAL;
using Microsoft.AspNetCore.Identity;

namespace JournalyApiV2.Services.BLL;

public class AuthService : IAuthService
{
    private readonly UserManager<JournalyUser> _userManager;
    private readonly IConfiguration _config;
    private readonly SignInManager<JournalyUser> _signInManager;
    private readonly IAuthDbService _authDbService;
    private readonly IEmailService _emailService;
    public AuthService(UserManager<JournalyUser> userManager, IConfiguration config, SignInManager<JournalyUser> signInManager, IAuthDbService authDbService, IEmailService emailService)
    {
        _userManager = userManager;
        _config = config;
        _signInManager = signInManager;
        _authDbService = authDbService;
        _emailService = emailService;
    }
    
    public async Task<AuthenticationResponse> SignIn(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new ArgumentException("Incorrect Email or Password");
        }
        var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
        if (result.Succeeded)
        {
            var token = await _authDbService.GenerateToken(Guid.Parse(user.Id));
            return new AuthenticationResponse
            {
                Token = token,
            };
        }
        else
        {
            throw new ArgumentException("Incorrect Email or Password");
        }
    }

    public async Task VoidToken(string token)
    {
        await _authDbService.RevokeToken(token);
    }
    
    public async Task CreateUser(string email, string password, string firstName, string lastName)
    {
        if (await _userManager.FindByEmailAsync(email) != null) throw new ArgumentException("Email already in use");
        var result = await _userManager.CreateAsync(new JournalyUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email
        }, password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("\n", result.Errors.Select(x => x.Description)));
        }

        var newUser = await _userManager.FindByEmailAsync(email);
        await VerifyEmail(Guid.Parse(newUser.Id), newUser.Email, newUser.FirstName, newUser.LastName);
    }

    public async Task ChangeName(string firstName, string lastName, Guid userId, int tokenId)
    {
        // Change name
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        user.FirstName = firstName;
        user.LastName = lastName;
        await _userManager.UpdateAsync(user);
    }

    public async Task ChangeEmail(string email, Guid userId)
    {
        // change email
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        user.Email = email;
        user.UserName = email;
        await _userManager.UpdateAsync(user);
    }

    public async Task ChangePassword(Guid userId, string oldPassword, string newPassword, int tokenId, bool signOutEverywhere = true)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded)
        {
            throw new ArgumentException("Password is incorrect");
        }

        if (signOutEverywhere)
        {
            await SignOutEverywhereAsync(userId, tokenId);
        }
    }

    public async Task VerifyEmail(Guid userId, string toEmail, string firstName, string lastName)
    {
        var codes = await _authDbService.GetOrCreateEmailVerificationCode(userId);
        await _emailService.SendVerificationEmailAsync(toEmail, firstName, lastName, codes);
    }

    public async Task VerifyEmailWithLongCode(string longCode)
    {
        var user = await _authDbService.GetUserByLongCode(longCode);

        if (user == null) throw new ArgumentException("Invalid verification code");

        await _authDbService.VerifyUser(user.Value);
        var userObj = await _userManager.FindByIdAsync(user.Value.ToString());

        userObj.EmailConfirmed = true;

        await _userManager.UpdateAsync(userObj);
    }

    public async Task VerifyEmailWithShortCode(Guid userId, string shortCode)
    {
        var result = await _authDbService.CheckShortCode(userId, shortCode);

        if (result)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new Exception("Short code was found, but not the associated user");
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            await _authDbService.VerifyUser(userId);
        }
        else
        {
            throw new ArgumentException("Invalid verification code");
        }
    }

    public async Task ResendVerificationEmailAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        await _authDbService.ResetEmailVerificationTimerAsync(userId);
        await VerifyEmail(userId, user.Email, user.FirstName, user.LastName);
    }

    public async Task ResetPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new ArgumentException("User not found");
        var code = await _authDbService.GetPasswordResetCode(Guid.Parse(user.Id));
        if (code == null)
        {
            code = await _authDbService.GeneratePasswordResetCode(Guid.Parse(user.Id));
        }
        else
        {
            await _authDbService.ResetPasswordResetTimerAsync(Guid.Parse(user.Id));
        }
        await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, user.LastName, code);
    }

    public async Task SubmitPasswordResetAsync(string code, string password, bool signOutEverywhere)
    {
        var userGuid = await _authDbService.LookupPasswordResetAsync(code);
        if (userGuid == null) throw new ArgumentException("Invalid password reset code");
        var user = await _userManager.FindByIdAsync(userGuid.Value.ToString());
        if (user == null) throw new Exception("User not found");
        await _userManager.RemovePasswordAsync(user);
        var result = await _userManager.AddPasswordAsync(user, password);
        if (result.Succeeded)
        {
            await _authDbService.ResetPassword(userGuid.Value);
        }
        else
        {
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description).ToArray()));
        }
        
        if (signOutEverywhere)
        {
            await SignOutEverywhereAsync(userGuid.Value);
        }
    }

    public async Task SignOutEverywhereAsync(Guid userId)
    {
        await _authDbService.RevokeTokens(userId);   
    }
    
}