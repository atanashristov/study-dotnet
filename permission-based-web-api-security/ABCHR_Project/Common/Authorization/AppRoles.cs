using System.Collections.ObjectModel;

namespace Common.Authorization
{
    // These are the default roles that are created with the application. We seed these to the DB.
    public static class AppRoles
    {
        public const string Admin = nameof(Admin);
        public const string Basic = nameof(Basic);

        public static IReadOnlyList<string> DefaultRoles { get; }
            = new ReadOnlyCollection<string>(new[]
            {
                Admin,
                Basic
            });
        public static bool IsDefault(string roleName)
            => DefaultRoles.Any(r => r == roleName);
    }
}
