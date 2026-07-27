// JRM.API/Controllers/RolesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JRM.API.Controllers.Base;
using JRM.Application.Common.DTOs;
using JRM.Application.Features.LookupServices.Commands;
using JRM.Application.Features.RolesServices.Commands;
using JRM.Application.Features.RolesServices.Commands.Delete;
using JRM.Application.Features.VendorServices.Commands;
using JRM.Domain.Entities;
using JRM.Infrastructure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JRM.API.Controllers
{

    public class RolesController : BaseApiController
    {
        private readonly JRMDBContext _dbContext;

        public RolesController(JRMDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<object>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<object>>>> GetList()
        {
            var roles = await _dbContext.Roles
                .AsNoTracking()
                .Select(r => new
                {
                    id = r.Id,
                    code = r.Code,
                    name = r.NameEn,
                    description = r.NameAr,
                    active = r.IsActive,
                    usersCount = _dbContext.Users.Count(u => u.RoleId == r.Id)
                })
                .OrderBy(r => r.name)
                .ToListAsync();

            return BaseResponseHandler(ApiResponses<IEnumerable<object>>.Success(roles, "Roles retrieved successfully."));
        }

        [HttpGet("options")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<object>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<object>>>> GetOptions()
        {
            var options = await _dbContext.Roles
                .AsNoTracking()
                .Where(r => r.IsActive)
                .Select(r => new { label = r.NameEn, value = r.Id })
                .OrderBy(r => r.label)
                .ToListAsync();

            return BaseResponseHandler(ApiResponses<IEnumerable<object>>.Success(options, "Role options retrieved successfully."));
        }

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

        [HttpPost("Create")]
        /* [Authorize(Policy = "AdminOnly")] */// Adjust policy name based on your auth
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateRoleCommand command)
          => BaseResponseHandler(await Mediator.Send(command));



        [HttpPut("Update")]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateRoleCommand command)
                => BaseResponseHandler(await Mediator.Send(command));

        
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponses<bool>>> Delete([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteRoleCommand(id)));


        [HttpPut("UpdateStatus")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateStatus([FromBody] UpdateRoleStatusCommand command)
           => BaseResponseHandler(await Mediator.Send(command));






    }
}