using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Emails;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders;
using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Orders;
using KinoDev.ApiGateway.Infrastructure.Models.Enums;
using KinoDev.ApiGateway.Infrastructure.Services;
using KinoDev.ApiGateway.WebApi.Models;
using KinoDev.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    [AllowAnonymous]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICookieResponseService _cookieResponseService;

        private readonly IMemoryCache _memoryCache;

        public OrdersController(IMediator mediator, ICookieResponseService cookieResponseService, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _cookieResponseService = cookieResponseService;
            _memoryCache = memoryCache;
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
                _cookieResponseService.AppendToCookieResponse(Response.Cookies, ResponseCookies.CookieOrderId, response.Id.ToString(), DateTime.UtcNow.AddMinutes(30));

                return Ok(response);
            }

            return BadRequest();
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedOrders()
        {
            var paidOrdersCookie = Request.Cookies[ResponseCookies.PaidOrderId];
            if (string.IsNullOrEmpty(paidOrdersCookie))
            {
                return Ok(Array.Empty<Guid>());
            }

            var orderIds = paidOrdersCookie
                .Split(new[] { ";", "%3B" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.Parse(id));

            var response = await _mediator.Send(new GetCompletedOrdersCommand
            {
                OrderIds = orderIds,
            });

            return Ok(response);
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
                _cookieResponseService.ResetCookie(Response.Cookies, ResponseCookies.CookieOrderId, orderId);

                // TODO: Move to service?
                var completedCookieValue = orderId;
                var existingPaidOrderId = Request.Cookies[ResponseCookies.PaidOrderId];
                if (existingPaidOrderId != null)
                {
                    completedCookieValue = existingPaidOrderId + ";" + orderId;
                }

                _cookieResponseService.AppendToCookieResponse(Response.Cookies, ResponseCookies.PaidOrderId, completedCookieValue, DateTime.UtcNow.AddMinutes(30));

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
                _cookieResponseService.ResetCookie(Response.Cookies, ResponseCookies.CookieOrderId, orderId);

                return Ok();
            }

            return BadRequest("Order not found or already completed.");
        }

        public class VerifyEmailModel
        {
            public string Email { get; set; }
        }

        [HttpPost("completed/verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailModel model)
        {
            var response = await _mediator.Send(new VerifyEmailCommand()
            {
                Email = model.Email,
                Reason = VerifyEmailReason.ConpletedOrdersRequest
            });

            if (response)
            {
                return Ok();
            }

            return BadRequest("Email verification failed.");
        }

        [HttpPost("completed/cookie")]
        public async Task<IActionResult> GetCompletedOrdersCookie([FromBody] GetCompletedOrderIdsByCodeVerifiedEmail model)
        {
            var orderIds = await _mediator.Send(new GetCompletedOrderIdsByCodeVerifiedEmail
            {
                Email = model.Email,
                Code = model.Code
            });

            if (!orderIds.IsNullOrEmptyCollection())
            {
                var completedCookieValue = string.Join(";", orderIds.Select(id => id.ToString()));
                _cookieResponseService.AppendToCookieResponse(Response.Cookies, ResponseCookies.PaidOrderId, completedCookieValue, DateTime.UtcNow.AddMinutes(30));
                return Ok();
            }

            return NotFound();
        }
    }
}
