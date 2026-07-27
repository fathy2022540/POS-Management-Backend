// JRM.Application/Common/DTOs/Role/RoleDto.cs
using System;

namespace JRM.Application.Common.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public int PermissionCount { get; set; }
    }
}