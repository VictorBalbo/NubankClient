using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NubankClient.NuHttp
{
    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public HttpClient()
        {
            _httpClient = new System.Net.Http.HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        public Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
            => SendRequest<T>(url, HttpMethod.Get, null, null, cancellationToken);

        public Task<T> GetWithAuthorizationAsync<T>(string url, string authorization, CancellationToken cancellationToken = default)
            => SendRequest<T>(url, HttpMethod.Get, null, authorization, cancellationToken);

        public Task<T> PostAsync<T>(string url, object body, CancellationToken cancellationToken = default)
            => SendRequest<T>(url, HttpMethod.Post, body, null, cancellationToken);

        public Task<T> PostWithAuthorizationAsync<T>(string url, object body, string authorization, CancellationToken cancellationToken = default)
            => SendRequest<T>(url, HttpMethod.Post, body, authorization, cancellationToken);

        private async Task<T> SendRequest<T>(string url, HttpMethod method, object body, string authorization = null, CancellationToken cancellationToken = default)
        {
            using (var requestMessage = new HttpRequestMessage(method, url))
            {
                if (!string.IsNullOrWhiteSpace(authorization))
                {
                    var authorizationHeader = GetAuthorizationHeader(authorization);
                    requestMessage.Headers.Add(authorizationHeader.Key, authorizationHeader.Value);
                }

                if (method == HttpMethod.Post)
                {
                    requestMessage.Content = new StringContent(JsonSerializer.Serialize(body));
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

                var responseStream = await response.Content.ReadAsStreamAsync();
                var stringResponse = await response.Content.ReadAsStringAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _serializerOptions, cancellationToken);
            }
        }

        private (string Key, string Value) GetAuthorizationHeader(string authToken)
        {
            return ("Authorization", $"Bearer {authToken}");
        }
    }
}