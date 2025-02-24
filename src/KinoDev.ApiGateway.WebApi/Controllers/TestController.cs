using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("hello")]
        public async Task<IActionResult> GetHello()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            // TODO: Allow it in local dev only
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };

            // TODO: Implement refresh-tokens
            // TODO: Move to service
            // TODO: Add DI for httClients
            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

            var response = httpClient.GetAsync("https://domain-service.kinodev.localhost/api/test/hello").Result;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok($"API gateway called domain service and got response: {content}");
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error calling domain service from API gateway");
            }
        }
    }
}
