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
        private const string authorityApiName = "AuthorityApi";
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public WebApiExecuter(
                IHttpClientFactory httpClientFactory,
                IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task<TResponse?> InvokeGetAsync<TResponse>(string relativeUrl)
        {
            var httpClient = await GetAuthenticatedHttpClientAsync();
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
            var httpClient = await GetAuthenticatedHttpClientAsync();

            var response = await httpClient.PostAsJsonAsync(relativeUrl, request);

            await EnsureSuccessStatusCodeAsync(response);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse?> InvokePutAsync<TRequest, TResponse>(
            string relativeUrl,
            TRequest request)
        {
            var httpClient = await GetAuthenticatedHttpClientAsync();

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
            var httpClient = await GetAuthenticatedHttpClientAsync();

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

        private async Task<HttpClient> GetAuthenticatedHttpClientAsync()
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeaders(httpClient);
            return httpClient;
        }

        private async Task AddJwtToHeaders(HttpClient httpClient)
        {
            var clientId = configuration.GetValue<string>("ClientId");
            var clientSecret = configuration.GetValue<string>("ClientSecret");

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("ClientId and ClientSecret must be configured.");
            }

            // Authenticate
            var authClient = httpClientFactory.CreateClient(authorityApiName);

            var response = await authClient.PostAsJsonAsync("auth", new AppCredentials
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            });

            await EnsureSuccessStatusCodeAsync(response);

            // Get JWT from authority
            string strToken = await response.Content.ReadAsStringAsync();
            var jwtToken = System.Text.Json.JsonSerializer.Deserialize<JwtToken>(strToken);

            // Add JWT to Authorization header
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken?.AccessToken);
        }
    }
}
