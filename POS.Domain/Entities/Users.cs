using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class Users : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string? UserName { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string? PasswordHash { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string? FullName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string? FirstName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string? LastName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public bool? IsLockedOut { get; set; }
        public DateTime? LastLockedOutDate { get; set; }
        public int? FailedPasswordAttemptCount { get; set; }
        public DateTime? FailedPasswordAttemptWindowStart { get; set; }
        public DateTime? PasswordDate { get; set; }

        public string? Pass1 { get; set; }
        public DateTime? PassDate1 { get; set; }
        public string? Pass2 { get; set; }
        public DateTime? PassDate2 { get; set; }
        public string? Pass3 { get; set; }
        public DateTime? PassDate3 { get; set; }

        public string? ActivationToken { get; set; }

        public bool? AccountProfileActivated { get; set; }
        public long? VendorId { get; set; }
        public long? RoleId { get; set; }
        public long? StatusId { get; set; }
        public long? UserTypeId { get; set; }


        [ForeignKey("RoleId")]
        public virtual Roles? Roles { get; set; }

        public virtual ICollection<UserScreenPermissionOverrides>? UserScreenPermissionOverrides { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }
        public virtual ICollection<WorkflowAction>? WorkflowActions { get; set; }
    }





}

