using System.Net;
using Application.Features.Identity.User.Commands;
using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity
{
    class RegisterUserOKResponseExample : IExamplesProvider<IResponseWrapper<string>>
    {
        public IResponseWrapper<string> GetExamples() => new ResponseWrapper<string> { IsSuccessful = true, ResponseData = Guid.NewGuid().ToString(), };
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController<UsersController>
    {
        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="userRegistration">User registration data</param>
        /// <returns>The ID of the registered user</returns>
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, "Returns the ID of the registered user", typeof(IResponseWrapper<string>))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(RegisterUserOKResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Email or username are taken", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Expired token", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Not authorized", Type = typeof(IResponseWrapper))]
        public async Task<ActionResult<IResponseWrapper<string>>> RegisterUser([FromBody] UserRegistrationRequest userRegistration)
        {
            var response = await MediatorSender
                .Send(new UserRegistrationCommand { UserRegistration = userRegistration });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{userId}")]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Returns the user", typeof(IResponseWrapper<UserResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User not found", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Expired token", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Not authorized", Type = typeof(IResponseWrapper))]
        public async Task<ActionResult<IResponseWrapper<UserResponse>>> GetUserById(string userId)
        {
            var response = await MediatorSender.Send(new GetUserByIdQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Returns the user", typeof(IResponseWrapper<List<UserResponse>>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Expired token", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Not authorized", Type = typeof(IResponseWrapper))]
        public async Task<ActionResult<IResponseWrapper<List<UserResponse>>>> GetAllUsers()
        {
            var response = await MediatorSender.Send(new GetAllUsersQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        [SwaggerResponse((int)HttpStatusCode.OK, "User details successfully updated", typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User not found", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Expired token", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Not authorized", Type = typeof(IResponseWrapper))]
        public async Task<ActionResult<IResponseWrapper>> UpdateUserDetails([FromBody] UpdateUserRequest updateUser)
        {
            var response = await MediatorSender.Send(new UpdateUserCommand { UpdateUser = updateUser });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("change-password")]
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, "User password updated", typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User not found, Incorrect password (which is actually not the right status...)", Type = typeof(IResponseWrapper))]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest changePassword)
        {
            var response = await MediatorSender
                .Send(new ChangeUserPasswordCommand { ChangePassword = changePassword });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("change-status")]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        [SwaggerResponse((int)HttpStatusCode.OK, "User status updated", typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User not found", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Expired token", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Not authorized", Type = typeof(IResponseWrapper))]
        public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest changeUserStatus)
        {
            var response = await MediatorSender
                .Send(new ChangeUserStatusCommand { ChangeUserStatus = changeUserStatus });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("roles/{userId}")]
        [MustHavePermission(AppFeature.Roles, AppAction.Read)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Returns list of user's roles", typeof(IResponseWrapper <List<UserRoleViewModel>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User not found", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Expired token", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Not authorized", Type = typeof(IResponseWrapper))]
        public async Task<ActionResult> GetRoles(string userId)
        {
            var response = await MediatorSender.Send(new GetRolesQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("user-roles")]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        [SwaggerResponse((int)HttpStatusCode.OK, "User roles  successfully updated", typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "User Roles update not permitted", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Expired token", Type = typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Not authorized", Type = typeof(IResponseWrapper))]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest updateUserRoles)
        {
            var response = await MediatorSender
                .Send(new UpdateUserRolesCommand { UpdateUserRoles = updateUserRoles });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
