using System.IdentityModel.Tokens.Jwt;
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
using JournalyApiV2.Services.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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

    private string GenerateJwtToken(string userId, string email, string givenName, string familyName, int tokenId)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, givenName),
            new Claim(JwtRegisteredClaimNames.FamilyName, familyName),
            new Claim("token_id", tokenId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Identity:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddSeconds(Convert.ToDouble(_config["Identity:ExpireSeconds"]));

        var token = new JwtSecurityToken(
            _config["Identity:Issuer"],
            _config["Identity:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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
            var refreshToken = await _authDbService.NewRefreshTokenAsync(Guid.Parse(user.Id));
            return new AuthenticationResponse
            {
                Token = GenerateJwtToken(user.Id, email, user.FirstName, user.LastName, refreshToken.TokenId),
                ExpiresIn = _config.GetValue<int>("Identity:ExpireSeconds"),
                RefreshToken = refreshToken.Token
            };
        }
        else
        {
            throw new ArgumentException("Incorrect Email or Password");
        }
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

    public async Task<AuthenticationResponse> RefreshToken(string refreshToken)
    {
        var owner = await _authDbService.LookupRefreshTokenAsync(refreshToken);
        if (owner == null)
        {
            throw new ArgumentException("Refresh token not valid");
        }

        var user = await _userManager.FindByIdAsync(owner.Value.ToString());
        if (user == null) throw new Exception("Refresh token is valid, but couldn't find user");
        var newToken = await _authDbService.ExchangeRefreshTokenAsync(refreshToken);
        if (newToken == null) throw new Exception("Failed to refresh token");
        return new AuthenticationResponse
        {
            RefreshToken = newToken.Token,
            ExpiresIn = _config.GetValue<int>("Identity:ExpireSeconds"),
            Token = GenerateJwtToken(user.Id, user.Email, user.FirstName, user.LastName, newToken.TokenId)
        };
    }

    public async Task<AuthenticationResponse> ChangeName(string firstName, string lastName, Guid userId, int tokenId)
    {
        // Change name
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        user.FirstName = firstName;
        user.LastName = lastName;
        await _userManager.UpdateAsync(user);
        
        // Generate new JWT and associated refresh token with the name updated
        await _authDbService.VoidRefreshTokensAsync(tokenId);
        var refreshToken = await _authDbService.NewRefreshTokenAsync(userId);
        var accessToken = GenerateJwtToken(userId.ToString(), user.Email, firstName, lastName, refreshToken.TokenId);
        
        return new AuthenticationResponse
        {
            ExpiresIn = _config.GetValue<int>("Identity:ExpireSeconds"),
            RefreshToken = refreshToken.Token,
            Token = accessToken
        };
    }

    public async Task<AuthenticationResponse> ChangeEmail(string email, Guid userId, int tokenId)
    {
        // change email
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        user.Email = email;
        user.UserName = email;
        await _userManager.UpdateAsync(user);
        
        // Generate new JWT and associated refresh token with the name updated
        await _authDbService.VoidRefreshTokensAsync(tokenId);
        var refreshToken = await _authDbService.NewRefreshTokenAsync(userId);
        var accessToken = GenerateJwtToken(userId.ToString(), email, user.FirstName, user.LastName, refreshToken.TokenId);
        
        return new AuthenticationResponse
        {
            ExpiresIn = _config.GetValue<int>("Identity:ExpireSeconds"),
            RefreshToken = refreshToken.Token,
            Token = accessToken
        };
    }

    public async Task ChangePassword(Guid userId, string oldPassword, string newPassword, int tokenId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new ArgumentException("User not found");
        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded)
        {
            throw new ArgumentException("Password is incorrect");
        }

        var existingRefreshTokens = await _authDbService.GetRefreshTokensAsync(userId);
        await _authDbService.VoidRefreshTokensAsync(existingRefreshTokens.Where(x => x.TokenId != tokenId).Select(x => x.TokenId).ToArray());
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
}