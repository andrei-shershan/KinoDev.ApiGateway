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

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentIntentModel model)
        {
            System.Console.WriteLine("!!!!!!!!!! " + model.Metadata?.Count());
            var response = await _mediator.Send(new CreatePaymentIntentCommand
            {
                Amount = model.Amount,
                Currency = model.Currency,
                Metadata = model.Metadata
            });

            if (response == null)
            {
                return BadRequest("HOHO!");
            }

            return Ok(new { clientSecret = response });
        }
    }
}