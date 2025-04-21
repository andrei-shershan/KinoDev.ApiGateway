using Newtonsoft.Json;

namespace KinoDev.ApiGateway.Infrastructure.Extensions
{
    public static class GenericResponseExtension
    {
        // TODO: Move to shared project
        public static async Task<T> GetResponseAsync<T>(this HttpResponseMessage response) where T: class
        {
            if (response == null)
            {
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
