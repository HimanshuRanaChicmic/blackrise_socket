namespace BlackRise.Identity.Application.Contracts;

public interface IHttpWrapper
{
    Task<T> GetAsync<T>(string url);
    Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData);
    Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest requestData);
    Task DeleteAsync(string url);
    void SetAuthorizationHeader(string token);
}
