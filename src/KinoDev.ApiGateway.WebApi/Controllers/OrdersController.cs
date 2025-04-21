using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders;
using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Orders;
using KinoDev.Shared.DtoModels.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    [AllowAnonymous]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class CreateOrderModel
        {
            public int ShowTimeId { get; set; }

            public ICollection<int> SelectedSeatIds { get; set; } = new List<int>();
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveOrder()
        {
            var orderId = Request.Cookies[ResponseCookies.CookieOrderId];
            if (orderId == null)
            {
                return NotFound();
            }

            var response = await _mediator.Send(new GetActiveOrderQuery()
            {
                OrderId = Guid.Parse(orderId),
            });

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderModel model)
        {
            var response = await _mediator.Send(new CreateOrderCommand()
            {
                ShowTimeId = model.ShowTimeId,
                SelectedSeatIds = model.SelectedSeatIds
            });

            if (response != null)
            {
                Response.Cookies.Append(
                    ResponseCookies.CookieOrderId,
                    response.Id.ToString(),
                    new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        //Domain = "localhost", // TODO: Env or Settings
                        Path = "/",
                        Expires = DateTime.UtcNow.AddMinutes(30)
                    });

                return Ok(response);
            }

            return BadRequest();
        }

        public class CompleteOrderModel
        {
            public string PaymentIntentId { get; set; }
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteOrder([FromBody] CompleteOrderModel model)
        {
            var orderId = Request.Cookies[ResponseCookies.CookieOrderId];
            if (orderId == null)
            {
                return BadRequest($"OrderId is required.");
            }

            var paymentIntentId = model.PaymentIntentId;
            if (string.IsNullOrWhiteSpace(paymentIntentId))
            {
                return BadRequest("PaymentIntentId is required.");
            }

            var response = await _mediator.Send(new CompleteOrderCommand()
            {
                OrderId = Guid.Parse(orderId),
                PaymentIntentId = paymentIntentId
            });

            if (response != null)
            {
                // TODO: Move to service?
                Response.Cookies.Append(
                    ResponseCookies.CookieOrderId,
                    orderId,
                    new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        //Domain = "localhost", // TODO: Env or Settings
                        Path = "/",
                        Expires = default(DateTime)
                    });

                // TODO: Move to service?
                var completedCookieValue = orderId;
                var existingPaidOrderId = Request.Cookies[ResponseCookies.PaidOrderId];
                if (existingPaidOrderId != null)
                {
                    completedCookieValue = existingPaidOrderId + ";" + orderId;
                }

                Response.Cookies.Append(
                    ResponseCookies.PaidOrderId,
                    completedCookieValue,
                    new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        //Domain = "localhost", // TODO: Env or Settings
                        Path = "/",
                        Expires = DateTime.UtcNow.AddMinutes(30)
                    });

                return Ok();
            }

            return BadRequest("Order not found or already completed.");
        }

        [HttpDelete("active")]
        public async Task<IActionResult> CancelActiveOrder()
        {
            var orderId = Request.Cookies[ResponseCookies.CookieOrderId];
            if (orderId == null)
            {
                return NotFound();
            }

            var response = await _mediator.Send(new CancelActiveOrderCommand()
            {
                OrderId = Guid.Parse(orderId),
            });

            if (response)
            {
                Response.Cookies.Append(
                    ResponseCookies.CookieOrderId,
                    orderId,
                    new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        //Domain = "localhost", // TODO: Env or Settings
                        Path = "/",
                        Expires = default(DateTime)
                    });                return Ok();
            }

            return BadRequest("Order not found or already completed.");
        }
    }
}
