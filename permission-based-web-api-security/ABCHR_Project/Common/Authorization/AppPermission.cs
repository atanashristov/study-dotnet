using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;

namespace Common.Authorization
{
    public record AppPermission(string Feature, string Action, string Group, string Description, bool IsBasic = false)
    {
        public string Name => NameFor(Feature, Action);

        private static string NameFor(string feature, string action)
        {
            return $"Permissions.{feature}.{action}";
        }
    }

    public class AppPermissions
    {
        private static readonly AppPermission[] _all = new AppPermission[]
        {
            new(AppFeature.Users, AppAction.Create, AppRoleGroup.SystemAccess, "Create Users"),
            new(AppFeature.Users, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users"),
            new(AppFeature.Users, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users"),
            new(AppFeature.Users, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Users"),

            // only able to view and update, not to create and delete. They will be created by the application
            new(AppFeature.UserRoles, AppAction.Read, AppRoleGroup.SystemAccess, "Read User Roles"),
            new(AppFeature.UserRoles, AppAction.Update, AppRoleGroup.SystemAccess, "Update User Roles"),

            new(AppFeature.Roles, AppAction.Read, AppRoleGroup.SystemAccess, "Read Roles"),
            new(AppFeature.Roles, AppAction.Create, AppRoleGroup.SystemAccess, "Create Roles"),
            new(AppFeature.Roles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Roles"),
            new(AppFeature.Roles, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Roles"),

            // only able to view and update, not to create and delete. They will be created by the application
            new(AppFeature.RoleClaims, AppAction.Read, AppRoleGroup.SystemAccess, "Read Role Claims/Permissions"),
            new(AppFeature.RoleClaims, AppAction.Update, AppRoleGroup.SystemAccess, "Update Role Claims/Permissions"),

            // IsBasic:
            // - When we create an user it gets only the basic permissions
            // - Any application role can read the employees, that's why this is a basic permission
            new(AppFeature.Employees, AppAction.Read, AppRoleGroup.ManagementHierarchy, "Read Employees", IsBasic: true),
            new(AppFeature.Employees, AppAction.Create, AppRoleGroup.ManagementHierarchy, "Create Employees"),
            new(AppFeature.Employees, AppAction.Update, AppRoleGroup.ManagementHierarchy, "Update Employees"),
            new(AppFeature.Employees, AppAction.Delete, AppRoleGroup.ManagementHierarchy, "Delete Employees")
        };

        public static IReadOnlyList<AppPermission> AdminPermissions { get; } =
            new ReadOnlyCollection<AppPermission>(_all.Where(p => !p.IsBasic).ToArray());

        public static IReadOnlyList<AppPermission> BasicPermissions { get; } =
            new ReadOnlyCollection<AppPermission>(_all.Where(p => p.IsBasic).ToArray());

        public static IReadOnlyList<AppPermission> AllPermissions { get; } =
            new ReadOnlyCollection<AppPermission>(_all);
    }
}
