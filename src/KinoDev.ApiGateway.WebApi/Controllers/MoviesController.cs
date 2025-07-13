using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Movies;
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
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ILogger<MoviesController> _logger;

        public MoviesController(ILogger<MoviesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        public async Task<IActionResult> GetMoviesAsync()
        {
            var response = await _mediator.Send(new GetMoviesQuery());
            if (response.IsNullOrEmptyCollection())
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        public async Task<IActionResult> GetMovieByIdAsync([FromRoute] int id)
        {
            var response = await _mediator.Send(new GetMovieByIdQuery()
            {
                Id = id
            });

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin}")]
        public async Task<IActionResult> CreateMovieAsync()
        {
            var response = await _mediator.Send(new CreateMovieCommand()
            {
                Form = Request.Form,
            });

            if (response == null)
            {
                return BadRequest("Failed to create movie.");
            }

            return CreatedAtAction(null, null, response);
        }

        [HttpGet("showing")]
        [AllowAnonymous]
        [OutputCache(Duration = 60, VaryByQueryKeys = new[] { "date" })]
        public async Task<IActionResult> GetShowingMoviesAsync([FromQuery] DateTime date)
        {
            var response = await _mediator.Send(new GetShowingMoviesQuery()
            {
                Date = date
            });

            if (response.IsNullOrEmptyCollection())
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
