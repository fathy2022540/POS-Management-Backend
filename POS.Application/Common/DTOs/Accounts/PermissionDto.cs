using System;
using System.Collections.Generic;

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

    public class SavePermissionMatrixRequest
    {
        public long RoleId { get; set; }
        public long? UserId { get; set; }
        public List<PermissionMatrixRowDto> Rows { get; set; } = [];
    }

    public class UserOptionDto
    {
        public string Label { get; set; } = string.Empty;
        public long Value { get; set; }
    }
}