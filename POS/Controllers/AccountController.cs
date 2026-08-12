using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.Authenticate;
using POS.Application.Features.UserAccount.Commands;
using POS.Application.Features.UsersAccount.Queries;
using POS.Domain.Enums;
using POS.Infrastructure;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly POSDBContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(POSDBContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        //[HttpPost("RegisterUserAccount")]
        //[AllowAnonymous]
        //[ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<ApiResponses<bool>>> Register([FromBody] RegisterVendorUserCommand command)
        //    => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("ActiveAccount")]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> ActivateAccount([FromBody] ActivateAccountCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponses<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<LoginResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponses<LoginResponse>>> Login([FromBody] LoginCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponses<RefreshTokenModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<RefreshTokenModel>>> RefreshToken([FromBody] RefreshTokenCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("UpdateProfile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateProfile([FromBody] UpdateProfileCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpGet("Users")]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<PaginationResponse<UserDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<PaginationResponse<UserDto>>>> GetAllUsers([FromQuery] GetAllUsersQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpGet("Users/{id:long}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponses<UserDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponses<UserDetailDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponses<UserDetailDto>>> GetUserById(long id)
            => BaseResponseHandler(await Mediator.Send(new GetUserByIdQuery { UserId = id }));

        [HttpGet("UsersManagement")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponses<IEnumerable<object>>>> GetUsersManagement([FromQuery] string? search = null)
        {
            var normalizedSearch = search?.Trim();
            var query = _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                query = query.Where(u =>
                    (u.FullName != null && EF.Functions.Like(u.FullName, $"%{normalizedSearch}%")) ||
                    (u.Email != null && EF.Functions.Like(u.Email, $"%{normalizedSearch}%")) ||
                    (u.UserName != null && EF.Functions.Like(u.UserName, $"%{normalizedSearch}%")) ||
                    (u.Roles != null && u.Roles.NameEn != null && EF.Functions.Like(u.Roles.NameEn, $"%{normalizedSearch}%")));


            }

            var users = await query
                .OrderByDescending(u => u.CreatedDate)
                .Select(u => new
                {
                    id = u.Id,
                    fullName = u.FullName,
                    username = u.UserName,
                    email = u.Email,
                    phone = u.Mobile,
                    roleId = u.RoleId,
                    role = u.Roles != null ? u.Roles.NameEn : string.Empty,
                    department = string.Empty,
                    isActive = u.IsActive,
                    documentName = string.Empty,
                    createdDate = u.CreatedDate
                })
                .ToListAsync();

            return BaseResponseHandler(ApiResponses<IEnumerable<object>>.Success(users, "Users retrieved successfully."));
        }

        [HttpPost("UsersManagement")]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponses<bool>>> CreateUserManagement([FromBody] UserManagementCommand command)
                  => BaseResponseHandler(await Mediator.Send(command));


        [HttpPut("UsersManagement/{id:long}")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateUserManagement(long id, [FromBody] UserManagementRequest request)
        {
            var normalizedRequest = NormalizeRequest(request);
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return BaseResponseHandler(ApiResponses<bool>.Failure(StatusResult.NotFound, "User not found."));
            }

            var duplicateExists = await _dbContext.Users.AnyAsync(u => u.Id != id && (u.UserName == normalizedRequest.Username || u.Email == normalizedRequest.Email));
            if (duplicateExists)
            {
                return BaseResponseHandler(ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "Username or email already exists."));
            }

            var (firstName, lastName) = SplitName(normalizedRequest.FullName);

            user.FullName = normalizedRequest.FullName;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.UserName = normalizedRequest.Username;
            user.Email = normalizedRequest.Email;
            user.Mobile = normalizedRequest.Phone;
            user.RoleId = normalizedRequest.RoleId;
            user.IsActive = normalizedRequest.IsActive;
            user.ModifiedDate = DateTime.UtcNow;
            user.ModifiedBy = 1;


            await _dbContext.SaveChangesAsync();
            return BaseResponseHandler(ApiResponses<bool>.Success(true, "User updated successfully."));
        }

        [HttpDelete("UsersManagement/{id:long}")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponses<bool>>> DeleteUserManagement(long id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return BaseResponseHandler(ApiResponses<bool>.Failure(StatusResult.NotFound, "User not found."));
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();



            return BaseResponseHandler(ApiResponses<bool>.Success(true, "User deleted successfully."));
        }

        private static UserManagementRequest NormalizeRequest(UserManagementRequest request)
        {
            request.FullName = request.FullName?.Trim() ?? string.Empty;
            request.Username = request.Username?.Trim() ?? string.Empty;
            request.Email = request.Email?.Trim() ?? string.Empty;
            request.Phone = request.Phone?.Trim() ?? string.Empty;
            request.UserType = string.Equals(request.UserType, "vendor", StringComparison.OrdinalIgnoreCase) ? "vendor" : "general";
            request.VendorName = request.VendorName?.Trim();
            request.CrNumber = request.CrNumber?.Trim();
            request.TaxNumber = request.TaxNumber?.Trim();
            request.TemporaryPassword = request.TemporaryPassword?.Trim();
            return request;
        }

        private static (string FirstName, string LastName) SplitName(string? fullName)
        {
            var names = (fullName ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = names.Length > 0 ? names[0] : string.Empty;
            var lastName = names.Length > 1 ? string.Join(' ', names.Skip(1)) : firstName;
            return (firstName, lastName);
        }


        public class UserManagementRequest
        {
            public string FullName { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public long? RoleId { get; set; }
            public bool IsActive { get; set; } = true;
            public string UserType { get; set; } = "general";
            public string? VendorName { get; set; }
            public string? CrNumber { get; set; }
            public string? TaxNumber { get; set; }
            public string? TemporaryPassword { get; set; }
        }
    }
}