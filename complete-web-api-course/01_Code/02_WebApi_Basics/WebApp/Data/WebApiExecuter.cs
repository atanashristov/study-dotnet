namespace WebApp.Data
{
    public interface IWebApiExecuter
    {
        Task<T?> GetAsync<T>(string relativeUrl);
    }

    public class WebApiExecuter : IWebApiExecuter
    {
        private const string apiName = "ShirtsApi";
        private readonly IHttpClientFactory httpClientFactory;

        public WebApiExecuter(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<T?> GetAsync<T>(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            return await httpClient.GetFromJsonAsync<T>(relativeUrl);
        }
    }
}
