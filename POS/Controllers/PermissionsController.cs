using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.Admission.PermissionServices.Queries;
using POS.Application.Features.PermissionServices.Commands;
using POS.Application.Features.PermissionServices.Queries;
using POS.Application.Features.RolesServices.Queries;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissionsController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponses<PaginationResponse<PermissionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<PaginationResponse<PermissionDto>>>> GetAll([FromQuery] GetAllPermissionsQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreatePermissionCommand command)
            => BaseResponseHandler(await Mediator.Send(command));


        [HttpPost("assign-to-role")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> AssignToRole([FromBody] AssignPermissionToRoleCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // GET: api/Permissions/roles-options
        [HttpGet("roles-options")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<RoleOptionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<RoleOptionDto>>>> GetRolesOptions()
            => BaseResponseHandler(await Mediator.Send(new GetRoleOptionsQuery()));

        // GET: api/Permissions/users-options
        [HttpGet("users-options")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<UserOptionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<UserOptionDto>>>> GetUsersOptions()
            => BaseResponseHandler(await Mediator.Send(new GetUserOptionsQuery()));

        // GET: api/Permissions/matrix?roleId=1&userId=2
        [HttpGet("matrix")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<PermissionMatrixRowDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<PermissionMatrixRowDto>>>> GetMatrix([FromQuery] long roleId, [FromQuery] long? userId)
            => BaseResponseHandler(await Mediator.Send(new GetPermissionMatrixQuery(roleId, userId)));

        // PUT: api/Permissions/matrix
        [HttpPut("matrix")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> SaveMatrix([FromBody] SavePermissionMatrixRequest request)
            => BaseResponseHandler(await Mediator.Send(new SavePermissionMatrixCommand(request)));
    }
}