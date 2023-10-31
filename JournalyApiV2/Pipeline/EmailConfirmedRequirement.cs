using JournalyApiV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JournalyApiV2.Pipeline;

public class EmailConfirmedRequirement : IAuthorizationRequirement { }

public class EmailConfirmedHandler : AuthorizationHandler<EmailConfirmedRequirement>
{
    private readonly UserManager<JournalyUser> _userManager;

    public EmailConfirmedHandler(UserManager<JournalyUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EmailConfirmedRequirement requirement)
    {
        var user = await _userManager.GetUserAsync(context.User);
        if (user != null && await _userManager.IsEmailConfirmedAsync(user))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}