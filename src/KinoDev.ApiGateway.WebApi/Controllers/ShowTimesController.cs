using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.ShowTimes;
using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.ShowTimes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    namespace KinoDev.ApiGateway.WebApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        [AllowAnonymous]
        public class ShowTimesController : ControllerBase
        {
            private readonly IMediator _mediator;

            public ShowTimesController(IMediator mediator)
            {
                _mediator = mediator;
            }

            [HttpGet("details/{id}")]
            public async Task<IActionResult> GetMoviesAsync([FromRoute] int id)
            {
                var response = await _mediator.Send(new GetShowTimeDetailsQuery()
                {
                    ShowTimeId = id
                });

                if (response != null)
                {
                    return Ok(response);
                }

                return NotFound();
            }

            [HttpGet("{startDate:datetime}/{endDate:datetime}")]
            public async Task<IActionResult> GetShowTimesByDateAsync([FromRoute] DateTime startDate, [FromRoute] DateTime endDate)
            {
                if (startDate > endDate)
                {
                    return BadRequest("Start date cannot be later than end date.");
                }

                var response = await _mediator.Send(new GetShowTimesByDateQuery()
                {
                    StartDate = startDate,
                    EndDate = endDate
                });

                if (response == null || !response.Any())
                {
                    return NotFound("No show times found for the specified date range.");
                }

                return Ok(response);
            }

            [HttpGet("seats/{id}")]
            public async Task<IActionResult> GetSeatsAsync([FromRoute] int id)
            {
                var response = await _mediator.Send(new GetShowTimeSeatsQuery()
                {
                    ShowTimeId = id
                });

                if (response != null)
                {
                    return Ok(response);
                }

                return NotFound();
            }

            // TODO: Add role 
            [Authorize]
            [HttpGet("slots/{date:datetime}")]
            public async Task<IActionResult> GetShowTimesForDateAsync([FromRoute] DateTime date)
            {
                var response = await _mediator.Send(new GetShowTimesForDateQuery()
                {
                    Date = date
                });

                if (response == null)
                {
                    return NotFound();
                }

                return Ok(response);
            }

            // TODO: Add role
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> CreateShowTimeAsync([FromBody] CreateShowTimeCommand command)
            {
                if (command == null)
                {
                    return BadRequest("Invalid show time data.");
                }

                var result = await _mediator.Send(command);
                if (result)
                {
                    return Created();
                }

                return BadRequest("Failed to create show time.");
            }            
        }
    }
}