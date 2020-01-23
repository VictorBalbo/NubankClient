using System.Threading;
using System.Threading.Tasks;

namespace NubankClient.Http
{
    public interface IHttpClient
    {
        Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default);
        Task<T> PostAsync<T>(string url, object body, CancellationToken cancellationToken = default);
        Task<T> GetWithAuthorizationAsync<T>(string url, string authorization, CancellationToken cancellationToken = default);
        Task<T> PostWithAuthorizationAsync<T>(string url, object body, string authorization, CancellationToken cancellationToken = default);
    }
}