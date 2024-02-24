namespace Common.Authorization
{
    public static class AppClaim
    {
        public const string Permission = "permission";

        // exp: for the JSON web token expiration time
        public const string Expiration = "exp";

        // We can add user full name, phone number etc, as needed...
    }
}
