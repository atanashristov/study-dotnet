using System.Net;
using Application.Features.Identity.Token.Queries;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    public class TokenController : BaseController<TokenController>
    {
        [HttpPost("get-token")]
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, "User details successfully updated", typeof(IResponseWrapper))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Wrong login, User deactivated, etc.", Type = typeof(IResponseWrapper))]
        public async Task<ActionResult<IResponseWrapper<TokenResponse>>> GetTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var response = await MediatorSender.Send(new GetTokenQuery { TokenRequest = tokenRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<IResponseWrapper<TokenResponse>>> GetRefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var response = await MediatorSender.Send(
                new GetRefreshTokenQuery { RefreshTokenRequest = refreshTokenRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
