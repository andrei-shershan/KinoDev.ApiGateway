using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Halls;
using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Halls;
using KinoDev.Shared.Constants;
using KinoDev.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // TODO: Provide read-only access for managers
    [Authorize]
    public class HallsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HallsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        public async Task<IActionResult> GetHallsAsync()
        {
            var halls = await _mediator.Send(new GetHallsQuery());
            if (halls.IsNullOrEmptyCollection())
            {
                return NotFound("No halls found.");
            }

            return Ok(halls);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        public async Task<IActionResult> GetHallByIdAsync([FromRoute] int id)
        {
            var hall = await _mediator.Send(new GetHallQuery { Id = id });
            if (hall == null)
            {
                return NotFound();
            }

            return Ok(hall);
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin}")]
        public async Task<IActionResult> CreateHallAsync([FromBody] CreateHallCommand request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            var createdHall = await _mediator.Send(request);
            if (createdHall == null)
            {
                return BadRequest("Failed to create hall.");
            }

            return CreatedAtAction(null, null, createdHall);
        }
    }
}