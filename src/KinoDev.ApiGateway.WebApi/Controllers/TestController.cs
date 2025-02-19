using Microsoft.AspNetCore.Mvc;

namespace KinoDev.ApiGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("hello")]
        public async Task<IActionResult> GetHello()
        {
            // TODO: Allow it in local dev only
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var httpClient = new HttpClient(handler);
            var response = httpClient.GetAsync("https://domain-service.kinodev.localhost/api/test/hello").Result;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok($"API gateway called domain service and got response: {content}");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error calling domain service from API gateway");
            }
        }
    }
}
