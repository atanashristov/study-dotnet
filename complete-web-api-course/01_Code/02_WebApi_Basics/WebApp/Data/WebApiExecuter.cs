namespace WebApp.Data
{
    public interface IWebApiExecuter
    {
        Task<T?> InvokeGetAsync<T>(string relativeUrl);
        Task<TResponse?> InvokePostAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request);
    }

    public class WebApiExecuter : IWebApiExecuter
    {
        private const string apiName = "ShirtsApi";
        private readonly IHttpClientFactory httpClientFactory;

        public WebApiExecuter(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<TResponse?> InvokeGetAsync<TResponse>(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            return await httpClient.GetFromJsonAsync<TResponse>(relativeUrl);
        }

        public async Task<TResponse?> InvokePostAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            var response = await httpClient.PostAsJsonAsync(relativeUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
    }
}
