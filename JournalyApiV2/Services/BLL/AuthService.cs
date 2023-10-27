using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using JournalyApiV2.Models;
using Microsoft.AspNetCore.Identity;

namespace JournalyApiV2.Services.BLL;

public class AuthService : IAuthService
{
    private readonly UserManager<JournalyUser> _userManager;
    public AuthService(UserManager<JournalyUser> userManager)
    {
        _userManager = userManager;
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