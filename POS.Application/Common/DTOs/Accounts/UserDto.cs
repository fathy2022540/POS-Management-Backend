// POS.Application/Common/DTOs/User/UserDto.cs
namespace POS.Application.Common.DTOs
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Active, Inactive, Suspended
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? LastLoginUtc { get; set; }
        public List<string> Roles { get; set; } = new();
    }
    public class UserModel
    {
        #region Properties
        public long? Id { get; set; }
        public string? FullName { get; set; }
        public string UserName { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PasswordQuestion { get; set; }
        public bool? IsApproved { get; set; } = true;
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastLockedOutDate { get; set; }
        public int? FailedPasswordAttemptCount { get; set; }
        public DateTime? FailedPasswordAttemptWindowStart { get; set; }
        public DateTime? PasswordDate { get; set; }

        public string Pass1 { get; set; }
        public DateTime? PassDate1 { get; set; }
        public string Pass2 { get; set; }
        public DateTime? PassDate2 { get; set; }
        public string Pass3 { get; set; }
        public DateTime? PassDate3 { get; set; }
        public DateTime? ExpiredOtp { get; set; }
        public string Otpcode { get; set; }

        public string Phone { get; set; }
        public string errorDetail { get; set; }
        #endregion


    }
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? LastLoginUtc { get; set; }
        public List<RoleDto> Roles { get; set; } = new();
        public List<PermissionDto> Permissions { get; set; } = new();
    }
    public class UserClaimModel
    {
        #region Properties

        public long? Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? entityTypeNameEn { get; set; }

        public string? CategoryCode { get; set; }

        public string? entityNameEn { get; set; }

        public string? phone_no { get; set; }

        public static UserClaimModel CreateUserClaimModel(UserModel userModel)
        {
            return new UserClaimModel
            {
                Id = userModel.Id,
                UserName = userModel.UserName,
                Email = userModel.Email

            };
        }

        #endregion
    }
}