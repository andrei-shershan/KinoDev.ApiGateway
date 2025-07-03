using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Payments;
using KinoDev.ApiGateway.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // TODO: Review authorization requirements
    // [Authorize]
    [AllowAnonymous]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IMediator mediator,
            ILogger<PaymentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest model)
        {
            var orderId = Request.Cookies[ResponseCookies.CookieOrderId];
            _logger.LogInformation("Received request to create payment for order ID: {OrderId}", orderId);
            if (orderId == null)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(new CreatePaymentIntentCommand
            {
                OrderId = Guid.Parse(orderId),
                Email = model.Email
            });

            if (response == null)
            {
                return BadRequest("Failed to create payment intent. Please try again later.");
            }

            return Ok(new { clientSecret = response });
        }
    }
}