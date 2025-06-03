using KinoDev.ApiGateway.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KinoDev.ApiGateway.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class PortalSettingsController : ControllerBase
{
    private readonly PortalSettings _portalSettings;

    public PortalSettingsController(IOptions<PortalSettings> portalSettings)
    {
        _portalSettings = portalSettings.Value;
    }

    [HttpGet]
    public ActionResult<PortalSettings> Get()
    {
        if (_portalSettings == null)
        {
            return NotFound("Portal settings not found.");
        }

        return Ok(_portalSettings);
    }
}