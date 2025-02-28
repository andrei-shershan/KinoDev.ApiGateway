namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public class HttpClientHandlerFactory
    {
        // TODO: Remove this when local issues with TLS for tgraefik solved
        public static HttpClientHandler CreateHandler(bool ignoreSslErrors)
        {
            var handler = new HttpClientHandler();

            // TODO: Add Env variable / settings
            if (ignoreSslErrors)
            {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }

            return handler;
        }
    }
}

