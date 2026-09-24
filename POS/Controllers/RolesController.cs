using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.RolesServices.Commands;
using POS.Application.Features.RolesServices.Commands.Delete;
using POS.Application.Features.RolesServices.Queries;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : BaseApiController
    {
        // GET: api/Roles/list
        [HttpGet("list")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<object>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<object>>>> GetList()
            => BaseResponseHandler(await Mediator.Send(new GetRolesListQuery()));

        // GET: api/Roles/options
        [HttpGet("options")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<RoleOptionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<RoleOptionDto>>>> GetOptions()
            => BaseResponseHandler(await Mediator.Send(new GetRoleOptionsQuery()));

        // GET: api/Roles
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponses<PaginationResponse<RoleDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<PaginationResponse<RoleDto>>>> GetAll([FromQuery] GetAllRolesQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        // GET: api/Roles/1
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ApiResponses<RoleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<RoleDto>>> GetById(long id)
            => BaseResponseHandler(await Mediator.Send(new GetRoleByIdQuery { Id = id }));

        // POST: api/Roles/Create
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateRoleCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // PUT: api/Roles/Update
        [HttpPut("Update")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateRoleCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // DELETE: api/Roles/Delete/1
        [HttpDelete("Delete/{id:long}")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> Delete([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteRoleCommand(id)));

        // PUT: api/Roles/UpdateStatus
        [HttpPut("UpdateStatus")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateStatus([FromBody] UpdateRoleStatusCommand command)
            => BaseResponseHandler(await Mediator.Send(command));


        //[HttpGet]
        //[ProducesResponseType(typeof(ApiResponses<PaginationResponse<RoleDto>>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponses<PaginationResponse<RoleDto>>), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<ApiResponses<PaginationResponse<RoleDto>>>> GetAll([FromQuery] GetAllRolesQuery query)
        //    => BaseResponseHandler(await Mediator.Send(query));

        //[HttpGet("{id:guid}")]
        //[ProducesResponseType(typeof(ApiResponses<RoleDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponses<RoleDto>), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<ApiResponses<RoleDto>>> GetById(Guid id)
        //    => BaseResponseHandler(await Mediator.Send(new GetRoleByIdQuery { RoleId = id }));
    }
}