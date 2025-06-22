using KinoDev.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        // TODO: Review this endpoint
        [HttpGet("details")]
        public IActionResult GetUserDetails()
        {
            var roles = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
            if (roles.IsNullOrEmptyCollection())
            {
                return NotFound("No roles found for the user.");
            }
            
            return Ok(roles);
        }
    }
}
