using System.Text.Json.Serialization;
using NubankClient.Models.Urls;

namespace NubankClient.Models.ApiResponses
{
    public class AuthenticationResponse : BaseResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("_links")]
        public AuthenticatedUrls Links { get; set; }        
    }
}