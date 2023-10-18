using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JournalyApiV2.Controllers;

[Authorize]
public class JournalyControllerBase : ControllerBase
{
    protected Guid GetUserId()
    { 
        return new Guid(
            HttpContext.User.Claims
                .Single(x => x.Type == ClaimTypes.NameIdentifier).Value
        );
    }
}