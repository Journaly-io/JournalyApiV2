using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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