using Common.Responses.Identity;
using System.Text.Json.Serialization;

namespace Common.Requests.Identity
{
    public class UpdateRolePermissionsRequest
    {
        public string RoleId { get; set; }
        public List<RoleClaimViewModel> RoleClaims { get; set; }
    }
}
