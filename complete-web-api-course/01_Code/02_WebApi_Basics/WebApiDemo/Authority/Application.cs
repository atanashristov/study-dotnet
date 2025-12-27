namespace WebApiDemo.Authority
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public string? ApplicationName { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Scopes { get; set; }
    }
}
