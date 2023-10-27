﻿using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace JournalyApiV2.Services.BLL;

public class AuthService : IAuthService
{
    private readonly UserManager<JournalyUser> _userManager;
    private readonly IConfiguration _config;
    private readonly SignInManager<JournalyUser> _signInManager;
    private readonly IDbFactory _db;
    public AuthService(UserManager<JournalyUser> userManager, IConfiguration config, SignInManager<JournalyUser> signInManager, IDbFactory db)
    {
        _userManager = userManager;
        _config = config;
        _signInManager = signInManager;
        _db = db;
    }

    private string GenerateJwtToken(string userId, string email, string givenName, string familyName)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, givenName),
            new Claim(JwtRegisteredClaimNames.FamilyName, familyName)
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
    
    private static string GenerateSecureOpaqueToken()
    {
        using var rng = new RNGCryptoServiceProvider();
        byte[] byteArr = new byte[32];
        rng.GetBytes(byteArr);
        return Convert.ToBase64String(byteArr);
    }

    private async Task<string> UpdateRefreshTokenAsync(Guid user)
    {
        using (var db = _db.Journaly())
        {
            db.RemoveRange(db.RefreshTokens.Where(x => x.UserId == user));
            var token = GenerateSecureOpaqueToken();
            await db.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user,
                Token = token
            });
            return token;
        }
    }
    
    public async Task<SignInResponse> SignIn(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("Email and password is valid, but no user found");
            }
            return new SignInResponse
            {
                Token = GenerateJwtToken(user.Id, email, user.FirstName, user.LastName),
                ExpiresIn = _config.GetValue<int>("Identity:ExpireSeconds"),
                RefreshToken = await UpdateRefreshTokenAsync(Guid.Parse(user.Id))
            };
        }
        else
        {
            throw new ArgumentException("Incorrect Email or Password");
        }
    }
    
    public async Task CreateUser(string email, string password, string firstName, string lastName)
    {
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
    }
}