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
using JournalyApiV2.Models.Requests;
using JournalyApiV2.Models.Responses;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.DAL;
using Microsoft.AspNetCore.Identity;
using EncryptedDEKType = JournalyApiV2.Data.Enums.EncryptedDEKType;

namespace JournalyApiV2.Services.BLL;

public class AuthService : IAuthService
{
    private readonly UserManager<JournalyUser> _userManager;
    private readonly IConfiguration _config;
    private readonly SignInManager<JournalyUser> _signInManager;
    private readonly IAuthDbService _authDbService;
    private readonly IEmailService _emailService;
    private readonly ICryptoDbService _cryptoDbService;
    public AuthService(UserManager<JournalyUser> userManager, IConfiguration config, SignInManager<JournalyUser> signInManager, IAuthDbService authDbService, IEmailService emailService, ICryptoDbService cryptoDbService)
    {
        _userManager = userManager;
        _config = config;
        _signInManager = signInManager;
        _authDbService = authDbService;
        _emailService = emailService;
        _cryptoDbService = cryptoDbService;
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

    public async Task<string> IssueToken(Guid userId)
    {
        return await _authDbService.GenerateToken(userId);
    }
    
    public async Task VoidToken(string token)
    {
        await _authDbService.RevokeToken(token);
    }
    
    public async Task CreateUser(string email, string password, string firstName, string lastName, string encryptedDEK, string KEKSalt)
    {
        if (await _userManager.FindByEmailAsync(email) != null) throw new ArgumentException("Email already in use");
        var result = await _userManager.CreateAsync(new JournalyUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,
            EmailConfirmed = email.EndsWith(".test")
        }, password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("\n", result.Errors.Select(x => x.Description)));
        }
        var newUser = await _userManager.FindByEmailAsync(email);
        await _cryptoDbService.StoreNewDEKForUser(Guid.Parse(newUser.Id), encryptedDEK, KEKSalt, EncryptedDEKType.Primary);
        if (!email.EndsWith(".test")) await VerifyEmail(Guid.Parse(newUser.Id), newUser.Email, newUser.FirstName, newUser.LastName);
    }

    public async Task ChangeName(string firstName, string lastName, Guid userId)
    {
        // Change name
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        user.FirstName = firstName;
        user.LastName = lastName;
        await _userManager.UpdateAsync(user);
    }

    public async Task ChangeEmail(string email, string passwordHash, Guid userId)
    {
        // change email
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        user.Email = email;
        user.UserName = email;
        await _userManager.UpdateAsync(user);
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, resetToken, passwordHash);
    }

    public async Task ChangePassword(Guid userId, string oldPassword, string newPassword, string encryptedDEK, string KEKSalt, string initiatorToken, bool signOutEverywhere = true)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        await _cryptoDbService.UpdateDEKForUser(userId, encryptedDEK, KEKSalt, EncryptedDEKType.Primary);
        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new ArgumentException("Password is incorrect");
        }

        if (signOutEverywhere)
        {
            await SignOutEverywhereAsync(userId, initiatorToken);
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
        await _authDbService.ClearEmailVerificationCodes(user.Value);
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
            await _authDbService.ClearEmailVerificationCodes(userId);
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

    public async Task SignOutEverywhereAsync(Guid userId, string initiatorToken)
    {
        await _authDbService.RevokeTokens(userId, new []{initiatorToken});   
    }

    public async Task<UserInfoResponse> GetUserInfoAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var dek = (await _cryptoDbService.GetDEKsForUser(userId, new[] { EncryptedDEKType.Primary })).Single();
        var hasRecoveryKeys = (await _authDbService.GetRecoveryKeys(userId)).Any();
        return new UserInfoResponse
        {
            Email = user.Email,
            EmailVerified = user.EmailConfirmed,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Uuid = userId,
            EncryptedDEK = dek.DEK,
            KEKSalt = dek.Salt,
            HasRecoveryKeys = hasRecoveryKeys
        };
    }

    public async Task BeginAccountRecovery(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return; // We will pretend this worked even if the email is not found;
        var codes = await _authDbService.GetOrCreateEmailVerificationCode(Guid.Parse(user.Id));
        await _emailService.SendAccountRecoveryEmailAsync(user.Email, user.FirstName, user.LastName, codes);
    }

    public async Task<string> IssueRecoveryTokenWithShortCode(string email, string shortCode)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new HttpBadRequestException("Invalid short code or short code does not match email");
        var userId = Guid.Parse(user.Id);
        if (!await _authDbService.CheckShortCode(userId, shortCode))
            throw new HttpBadRequestException("Invalid short code or short code does not match email");
        await _authDbService.ClearEmailVerificationCodes(userId);
        return await _authDbService.IssueRecoveryToken(userId);
    }

    public async Task<string> IssueRecoveryTokenWithLongCode(string longCode)
    {
        var user = await _authDbService.GetUserByLongCode(longCode);
        if (user == null) throw new HttpBadRequestException("Long code not found");
        return await _authDbService.IssueRecoveryToken(user.Value);
    }

    public async Task<CryptographicKey[]> GetRecoveryKeys(string recoveryToken)
    {
        return await _authDbService.GetRecoveryKeys(recoveryToken);
    }

    public async Task RecoverAccount(Guid userId, string passwordHash, CryptographicKey primaryKey)
    {
        if (primaryKey.Type != 1) throw new ArgumentException("Key must be a primary key");
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("UserID invalid");
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, resetToken, passwordHash);
        await _cryptoDbService.UpdateDEKForUser(userId, primaryKey.DEK, primaryKey.Salt, EncryptedDEKType.Primary);
        await _authDbService.ClearRecoveryTokens(userId);
    }

    public async Task<Guid?> GetUserIdFromRecoveryToken(string recoveryToken)
    {
        return await _authDbService.GetUserIdFromRecoveryToken(recoveryToken);
    }
}