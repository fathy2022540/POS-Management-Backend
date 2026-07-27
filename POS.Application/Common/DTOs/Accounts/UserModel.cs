namespace JRM.Application.Common.DTOs
{
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

}
