﻿namespace KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings
{
    public class ApiClientsSettings
    {
        public string DomainServiceUri { get; set; }
        
        public string IdentityServiceUri { get; set; }

        public string PaymentServiceUri { get; set; }
    }
}
