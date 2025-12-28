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
                ClientId = "ef9073b5-06d6-438e-a8c3-e6e76170dfca",
                ClientSecret = "x8h3pS6hYkUu9n5Z",
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
