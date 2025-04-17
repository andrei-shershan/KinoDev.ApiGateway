using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies;
using KinoDev.Shared.Constants;
using KinoDev.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.Admin}")]
    // [AllowAnonymous]
    public class MoviesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MoviesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMoviesAsync()
        {
            var response = await _mediator.Send(new GetMoviesQuery());

            if (!response.IsNullOrEmptyCollection())
            {
                return Ok(response);
            }

            return NotFound();
        }

        [HttpGet("showing")]
        [AllowAnonymous]
        //[OutputCache(Duration = 60, VaryByQueryKeys = new[] { "date" })]
        public async Task<IActionResult> GetShowingMoviesAsync([FromQuery] DateTime date)
        {
            var response = await _mediator.Send(new GetShowingMoviesQuery()
            {
                Date = date
            });

            if (!response.IsNullOrEmptyCollection())
            {
                Response.Cookies.Append("TEST_CASE", $"{DateTime.UtcNow.Millisecond}", new CookieOptions
                {
                    HttpOnly = true,
                    //Domain = "localhost", // TODO: Env or Settings
                    Path = "/",
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    //Expires = DateTime.UtcNow.AddMinutes(30)
                });

                return Ok(response);
            }

            return NotFound();
        }
    }
}
