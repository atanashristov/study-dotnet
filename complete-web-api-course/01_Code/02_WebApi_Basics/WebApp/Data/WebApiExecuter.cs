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
            return await httpClient.GetFromJsonAsync<TResponse>(relativeUrl);
        }

        public async Task<TResponse?> InvokePostAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            var response = await httpClient.PostAsJsonAsync(relativeUrl, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var exception = new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).");
                exception.Data["StatusCode"] = response.StatusCode;
                exception.Data["ResponseContent"] = errorContent;
                throw exception;
            }

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse?> InvokePutAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            var response = await httpClient.PutAsJsonAsync(relativeUrl, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var exception = new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).");
                exception.Data["StatusCode"] = response.StatusCode;
                exception.Data["ResponseContent"] = errorContent;
                throw exception;
            }

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

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var exception = new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).");
                exception.Data["StatusCode"] = response.StatusCode;
                exception.Data["ResponseContent"] = errorContent;
                throw exception;
            }
        }
    }
}
