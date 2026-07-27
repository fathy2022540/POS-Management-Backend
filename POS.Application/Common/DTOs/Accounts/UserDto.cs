// JRM.Application/Common/DTOs/User/UserDto.cs
using System;

namespace JRM.Application.Common.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Active, Inactive, Suspended
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? LastLoginUtc { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}