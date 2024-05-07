using System.Security.Claims;
using System.Text.Encodings.Web;
using JournalyApiV2.Services.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace JournalyApiV2.Pipeline;

public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IAuthDbService _tokenDbService;

    public CustomAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IAuthDbService authDbService)
        : base(options, logger, encoder, clock)
    {
        _tokenDbService = authDbService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var token))
        {
            return AuthenticateResult.Fail("No token provided.");
        }

        var userId = await _tokenDbService.ValidateToken(token);
        if (userId != null)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        else
        {
            return AuthenticateResult.Fail("Invalid token.");
        }
    }
}
