using KinoDev.ApiGateway.Infrastructure.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("mediatr")]
        public async Task<IActionResult> MediatrCall()
        {
            var result = await _mediator.Send(new GetInternalTestDataQuery() { });

            return Ok(result);
        }   
    }
}
