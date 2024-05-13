using System.Security.Claims;
using JournalyApiV2.Pipeline;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace JournalyApiV2.Controllers;

[Authorize(Policy = "EmailConfirmed")]
public class JournalyControllerBase : ControllerBase
{
    protected Guid GetUserId()
    {
        try
        {
            return new Guid(
                HttpContext.User.Claims
                    .Single(x => x.Type == ClaimTypes.NameIdentifier).Value
            );
        }
        catch (InvalidOperationException ex)
        {
            throw new HttpAccessDeniedException("Getting user ID failed", ex);
        }
    }

    protected Guid GetDeviceId()
    {
        StringValues deviceIdRaw;
        Guid deviceId;
        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out deviceIdRaw))
        {
            throw new ArgumentException("You must specify a device ID for this action");
        }

        if (deviceIdRaw.Count != 1)
        {
            throw new ArgumentException("Exactly 1 device ID can be specified");
        }

        if (!Guid.TryParse(deviceIdRaw[0], out deviceId))
        {
            throw new ArgumentException("Device ID not understood");
        }

        return deviceId;
    }
}