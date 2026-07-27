// JRM.Application/Common/DTOs/ParameterConfig/ParameterConfigDto.cs
using System;

namespace JRM.Application.Common.DTOs
{
    public class ParameterConfigDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DataType { get; set; } = string.Empty; // string, int, bool, decimal
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}