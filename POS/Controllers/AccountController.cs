using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.Admission.UserAccount.Commands;
using POS.Application.Features.Admission.UserAccount.Queries;
using POS.Application.Features.Authenticate;
using POS.Application.Features.UserAccount.Commands;
using POS.Application.Features.UsersAccount.Queries;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        // POST: api/Account/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponses<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<LoginResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponses<LoginResponse>>> Login([FromBody] LoginCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // POST: api/Account/RegisterUserAccount
        //[HttpPost("RegisterUserAccount")]
        //[AllowAnonymous]
        //[ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<ApiResponses<bool>>> Register([FromBody] RegisterVendorUserCommand command)
        //    => BaseResponseHandler(await Mediator.Send(command));

        // POST: api/Account/RefreshToken
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponses<RefreshTokenModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<RefreshTokenModel>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponses<RefreshTokenModel>>> RefreshToken([FromBody] RefreshTokenCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // PUT: api/Account/ActiveAccount
        [HttpPut("ActiveAccount")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> ActivateAccount([FromBody] ActivateAccountCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // PUT: api/Account/UpdateProfile
        [HttpPut("UpdateProfile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateProfile([FromBody] UpdateProfileCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // GET: api/Account/Users
        [HttpGet("Users")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<PaginationResponse<UserDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<PaginationResponse<UserDto>>>> GetAllUsers([FromQuery] GetAllUsersQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        // GET: api/Account/Users/{id}
        [HttpGet("Users/{id:long}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponses<UserDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<UserDetailDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponses<UserDetailDto>>> GetUserById(long id)
            => BaseResponseHandler(await Mediator.Send(new GetUserByIdQuery { UserId = id }));

        // GET: api/Account/UsersManagement
        [HttpGet("UsersManagement")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<object>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<object>>>> GetUsersManagement([FromQuery] string? search = null)
            => BaseResponseHandler(await Mediator.Send(new GetUsersManagementQuery { Search = search }));

        // POST: api/Account/UsersManagement
        [HttpPost("UsersManagement")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponses<bool>>> CreateUserManagement([FromBody] UserManagementCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // PUT: api/Account/UsersManagement/{id}
        [HttpPut("UsersManagement/{id:long}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateUserManagement(long id, [FromBody] UpdateUserManagementCommand command)
            => BaseResponseHandler(await Mediator.Send(command with { Id = id }));

        // DELETE: api/Account/UsersManagement/{id}
        [HttpDelete("UsersManagement/{id:long}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> DeleteUserManagement(long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteUserManagementCommand(id)));
    }
}