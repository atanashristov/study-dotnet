namespace WebApp.Data
{
    public interface IWebApiExecuter
    {
        Task<T?> InvokeGetAsync<T>(string relativeUrl);
        Task<TResponse?> InvokePostAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request);
        Task<TResponse?> InvokePutAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request);
        Task InvokeDeleteAsync(string relativeUrl);
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
            // var response = await httpClient.GetAsync(relativeUrl);

            // Another way to do it:
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            var response = await httpClient.SendAsync(request);

            await EnsureSuccessStatusCodeAsync(response);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse?> InvokePostAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            var response = await httpClient.PostAsJsonAsync(relativeUrl, request);

            await EnsureSuccessStatusCodeAsync(response);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse?> InvokePutAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            var response = await httpClient.PutAsJsonAsync(relativeUrl, request);

            await EnsureSuccessStatusCodeAsync(response);

            // Handle 204 No Content responses
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent ||
                response.Content.Headers.ContentLength == 0)
            {
                return default(TResponse);
            }

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task InvokeDeleteAsync(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            var response = await httpClient.DeleteAsync(relativeUrl);

            await EnsureSuccessStatusCodeAsync(response);
        }

        private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new WebApiException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).", errorContent, response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}
