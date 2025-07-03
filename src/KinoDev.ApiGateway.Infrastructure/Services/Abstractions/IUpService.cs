namespace KinoDev.ApiGateway.Infrastructure.Services.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for UP service.
    /// </summary>
    public interface IUpService
    {
        /// <summary>
        /// Checks if the service is up and running.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Up();
    }
}