namespace KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings
{
    public class AuthenticationSettings
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public Audience Audiences { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }

    public class Audience
    {
        public string Gateway { get; set; }
    }
}
