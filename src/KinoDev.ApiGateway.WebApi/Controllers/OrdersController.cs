using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders;
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

        [HttpPost]
        public async Task<IActionResult> GetUserDetails([FromBody] CreateOrderModel model)
        {
            var response = await _mediator.Send(new CreateOrderCommand()
            {
                ShowTimeId = model.ShowTimeId,
                SelectedSeatIds = model.SelectedSeatIds
            });

            if (response != null)
            {
                Response.Cookies.Append(
                    "order_cookie",
                    response.Id.ToString(),
                    new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Domain = "localhost",
                        Path = "/"
                    });

                return Ok(response);
            }

            return BadRequest();
        }
    }
}
