namespace WebApiDemo.Authority
{
    public static class AppRepository
    {
        private static readonly List<Application> _applications = new()
        {
            new Application
            {
                ApplicationId = 1,
                ApplicationName = "WebApp",
                ClientId = "client-id",
                ClientSecret = "client-secret",
                // Scopes = "read:shirts write:shirts",
                Scopes = "read,write,delete",
            }
        };

        public static Application? GetApplicationByClientId(string clientId)
        {
            return _applications.FirstOrDefault(app => app.ClientId == clientId);
        }
    }
}
