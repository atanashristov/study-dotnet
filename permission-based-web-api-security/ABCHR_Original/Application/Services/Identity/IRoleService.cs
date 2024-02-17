using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity
{
    public interface IRoleService
    {
        Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request);
        Task<IResponseWrapper> GetRolesAsync();
        Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request);
        Task<IResponseWrapper> GetRoleByIdAsync(string roleId);
        Task<IResponseWrapper> DeleteRoleAsync(string roleId);
        Task<IResponseWrapper> GetPermissionsAsync(string roleId);
        Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request);
    }
}
