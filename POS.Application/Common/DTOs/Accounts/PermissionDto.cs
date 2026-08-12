// POS.Application/Common/DTOs/Permission/PermissionDto.cs
using System;

namespace POS.Application.Common.DTOs
{
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Resource { get; set; } = string.Empty; // e.g., "Vendor", "Product", "Report"
        public string Action { get; set; } = string.Empty;   // e.g., "Create", "Read", "Update", "Delete"
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}