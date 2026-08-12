// POS.API/Controllers/PermissionsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.PermissionServices.Queries;
using POS.Application.Features.PermissionServices.Commands;
using POS.Domain.Entities;
using POS.Infrastructure;

namespace POS.API.Controllers
{

    public class PermissionsController : BaseApiController
    {
        private readonly POSDBContext _dbContext;

        public PermissionsController(POSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponses<PaginationResponse<PermissionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<PaginationResponse<PermissionDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponses<PaginationResponse<PermissionDto>>>> GetAll([FromQuery] GetAllPermissionsQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpPost]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreatePermissionCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPost("assign-to-role")]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> AssignToRole([FromBody] AssignPermissionToRoleCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpGet("roles-options")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<object>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<object>>>> GetRolesOptions()
        {
            var roles = await _dbContext.Roles
                .AsNoTracking()
                .Where(r => r.IsActive)
                .Select(r => new { label = r.NameEn, value = r.Id })
                .OrderBy(r => r.label)
                .ToListAsync();

            return BaseResponseHandler(ApiResponses<IEnumerable<object>>.Success(roles, "Role options retrieved successfully."));
        }

        [HttpGet("users-options")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<object>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<object>>>> GetUsersOptions()
        {
            var users = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.IsActive == true)
                .Select(u => new { label = u.FullName + " (" + u.UserName + ")", value = u.Id })
                .OrderBy(u => u.label)
                .ToListAsync();

            return BaseResponseHandler(ApiResponses<IEnumerable<object>>.Success(users, "User options retrieved successfully."));
        }

        [HttpGet("matrix")]
        [ProducesResponseType(typeof(ApiResponses<IEnumerable<PermissionMatrixRowDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<IEnumerable<PermissionMatrixRowDto>>>> GetMatrix([FromQuery] long roleId, [FromQuery] long? userId)
        {
            var screens = await _dbContext.SystemScreens
                .AsNoTracking()
                .Include(s => s.Module)
                .OrderBy(s => s.Module!.SortOrder)
                .ThenBy(s => s.SortOrder)
                .ToListAsync();

            var rolePerms = await _dbContext.RoleScreenPermissions
                .AsNoTracking()
                .Where(x => x.RoleId == roleId)
                .ToDictionaryAsync(x => x.ScreenId, x => x);

            var userOverrides = new Dictionary<long, UserScreenPermissionOverrides>();
            if (userId.HasValue)
            {
                userOverrides = await _dbContext.UserScreenPermissionOverrides
                    .AsNoTracking()
                    .Where(x => x.UserId == userId.Value)
                    .ToDictionaryAsync(x => x.ScreenId, x => x);
            }

            var rows = screens.Select(s =>
            {
                rolePerms.TryGetValue(s.Id, out var basePerm);
                userOverrides.TryGetValue(s.Id, out var ov);

                return new PermissionMatrixRowDto
                {
                    ScreenId = s.Id,
                    ScreenCode = s.ScreenCode,
                    ScreenNameEn = s.ScreenName,
                    ScreenNameAr = s.ScreenNameAR,
                    Module = s.Module != null ? s.Module.Name : string.Empty,
                    View = ov?.CanView ?? basePerm?.CanView ?? false,
                    Create = ov?.CanCreate ?? basePerm?.CanCreate ?? false,
                    Edit = ov?.CanEdit ?? basePerm?.CanEdit ?? false,
                    Delete = ov?.CanDelete ?? basePerm?.CanDelete ?? false,
                    Approve = ov?.CanApprove ?? basePerm?.CanApprove ?? false,
                    Export = ov?.CanExport ?? basePerm?.CanExport ?? false
                };
            }).ToList();

            return BaseResponseHandler(ApiResponses<IEnumerable<PermissionMatrixRowDto>>.Success(rows, "Permission matrix retrieved successfully."));
        }

        [HttpPut("matrix")]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> SaveMatrix([FromBody] SavePermissionMatrixRequest request)
        {
            if (request.UserId.HasValue)
            {
                var rolePerms = await _dbContext.RoleScreenPermissions
                    .AsNoTracking()
                    .Where(x => x.RoleId == request.RoleId)
                    .ToDictionaryAsync(x => x.ScreenId, x => x);

                var existingOverrides = await _dbContext.UserScreenPermissionOverrides
                    .Where(x => x.UserId == request.UserId.Value)
                    .ToListAsync();

                _dbContext.UserScreenPermissionOverrides.RemoveRange(existingOverrides);

                var overrides = request.Rows
                    .Where(r =>
                    {
                        rolePerms.TryGetValue(r.ScreenId, out var rolePerm);
                        return (rolePerm?.CanView ?? false) != r.View ||
                               (rolePerm?.CanCreate ?? false) != r.Create ||
                               (rolePerm?.CanEdit ?? false) != r.Edit ||
                               (rolePerm?.CanDelete ?? false) != r.Delete ||
                               (rolePerm?.CanApprove ?? false) != r.Approve ||
                               (rolePerm?.CanExport ?? false) != r.Export;
                    })
                    .Select(r => new UserScreenPermissionOverrides
                    {
                        UserId = request.UserId.Value,
                        ScreenId = r.ScreenId,
                        CanView = r.View,
                        CanCreate = r.Create,
                        CanEdit = r.Edit,
                        CanDelete = r.Delete,
                        CanApprove = r.Approve,
                        CanExport = r.Export,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1
                    }).ToList();

                await _dbContext.UserScreenPermissionOverrides.AddRangeAsync(overrides);
            }
            else
            {
                var rolePerms = await _dbContext.RoleScreenPermissions
                    .Where(x => x.RoleId == request.RoleId)
                    .ToListAsync();

                _dbContext.RoleScreenPermissions.RemoveRange(rolePerms);

                var newRolePerms = request.Rows.Select(r => new RoleScreenPermissions
                {
                    RoleId = request.RoleId,
                    ScreenId = r.ScreenId,
                    CanView = r.View,
                    CanCreate = r.Create,
                    CanEdit = r.Edit,
                    CanDelete = r.Delete,
                    CanApprove = r.Approve,
                    CanExport = r.Export,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1
                }).ToList();

                await _dbContext.RoleScreenPermissions.AddRangeAsync(newRolePerms);
            }

            await _dbContext.SaveChangesAsync();
            return BaseResponseHandler(ApiResponses<bool>.Success(true, "Permission matrix saved successfully."));
        }

        [HttpDelete("matrix")]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> DeleteMatrix([FromQuery] long roleId, [FromQuery] long? userId)
        {
            if (userId.HasValue)
            {
                var overrides = await _dbContext.UserScreenPermissionOverrides
                    .Where(x => x.UserId == userId.Value)
                    .ToListAsync();

                _dbContext.UserScreenPermissionOverrides.RemoveRange(overrides);
            }
            else
            {
                var rolePerms = await _dbContext.RoleScreenPermissions
                    .Where(x => x.RoleId == roleId)
                    .ToListAsync();

                _dbContext.RoleScreenPermissions.RemoveRange(rolePerms);
            }

            await _dbContext.SaveChangesAsync();
            return BaseResponseHandler(ApiResponses<bool>.Success(true, "Permission matrix deleted successfully."));
        }

        public class SavePermissionMatrixRequest
        {
            public long RoleId { get; set; }
            public long? UserId { get; set; }
            public List<PermissionMatrixRowDto> Rows { get; set; } = [];
        }

        public class PermissionMatrixRowDto
        {
            public long ScreenId { get; set; }
            public string ScreenCode { get; set; } = string.Empty;
            public string ScreenNameEn { get; set; } = string.Empty;
            public string? ScreenNameAr { get; set; }
            public string Module { get; set; } = string.Empty;
            public bool View { get; set; }
            public bool Create { get; set; }
            public bool Edit { get; set; }
            public bool Delete { get; set; }
            public bool Approve { get; set; }
            public bool Export { get; set; }
        }
    }
}