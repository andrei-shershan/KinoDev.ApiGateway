using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    [AllowAnonymous]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class CreatePaymentIntentModel
        {
            public int Amount { get; set; }

            public string Currency { get; set; }

            public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePayment()
        {
            var orderId = Request.Cookies[ResponseCookies.CookieOrderId];
            if (orderId == null)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(new CreatePaymentIntentCommand
            {
                OrderId = Guid.Parse(orderId)
            });

            if (response == null)
            {
                return BadRequest("HOHO!");
            }

            return Ok(new { clientSecret = response });
        }
    }
}