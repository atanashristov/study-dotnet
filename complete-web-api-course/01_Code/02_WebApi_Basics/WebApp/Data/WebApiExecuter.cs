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
        private readonly IHttpContextAccessor httpContextAccessor;

        public WebApiExecuter(
                    IHttpClientFactory httpClientFactory,
                    IConfiguration configuration,
                    IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
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
            JwtToken? existingToken = null;
            var tokenString = this.httpContextAccessor.HttpContext?.Session.GetString("access_token");
            if (!string.IsNullOrWhiteSpace(tokenString))
            {
                existingToken = System.Text.Json.JsonSerializer.Deserialize<JwtToken>(tokenString);
            }
            if (existingToken != null && existingToken.ExpiresAt > DateTime.UtcNow.AddMinutes(5))
            {
                // Token is still valid, use it
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", existingToken.AccessToken);
                return;
            }

            var clientId = configuration.GetValue<string>("ClientId");
            var clientSecret = configuration.GetValue<string>("ClientSecret");

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
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

            this.httpContextAccessor.HttpContext?.Session.SetString("access_token", strToken);

            // Add JWT to Authorization header
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken?.AccessToken);
        }
    }
}
