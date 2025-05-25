using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Halls;
using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Halls;
using KinoDev.Shared.DtoModels.Hall;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HallsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetHallsAsync()
        {
            var halls = await _mediator.Send(new GetHallsQuery());
            return Ok(halls);
        }

        [HttpGet("{id:int}")]
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
        public async Task<IActionResult> CreateHallAsync([FromBody] HallDto hallDto)
        {
            if (hallDto == null)
            {
                return BadRequest("Hall data is required.");
            }

            var createdHall = await _mediator.Send(new CreateHallCommand { Hall = hallDto });
            return CreatedAtAction(null, null, createdHall);
        }
    }
}