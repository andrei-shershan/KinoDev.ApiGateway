﻿using Newtonsoft.Json;

namespace KinoDev.ApiGateway.Infrastructure.Models
{
    public class TokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expired_at")]
        public DateTime ExpiredAt { get; set; }
    }
}
