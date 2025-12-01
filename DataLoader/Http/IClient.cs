using System.Diagnostics.CodeAnalysis;

namespace DataLoader.Http
{
    public interface IClient
    {
        Task<string> GetHeaderValue(string url, string headerKey);
        Task<T> Get<T>(string url, int retry = 0);

        Task<T> Post<T, R>(string url, R query);
        Task<T> Put<T, R>(string url, R query);
        Task<T> Patch<T>(string url, object query);
        Task<bool> Delete(string url);

        Task<HttpResponseMessage> PutAsync([StringSyntax("Uri")] string? requestUri, HttpContent? content);
        Task<HttpResponseMessage> GetAsync([StringSyntax("Uri")] string? requestUri);
    }
}
