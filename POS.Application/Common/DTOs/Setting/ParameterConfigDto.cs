// POS.Application/Common/DTOs/ParameterConfig/ParameterConfigDto.cs
namespace POS.Application.Common.DTOs
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
    public class ParamterConfigListRespone
    {
        public long Id { get; set; }
        public string Keyword { get; set; }
        public string? ContentAr { get; set; }
        public string? Parent { get; set; }
    }
    public class paramterConfigRespone
    {
        public long Id { get; set; }
        public string Keyword { get; set; }
        public string? Parent { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ContentEn { get; set; }
        public string? ContentAr { get; set; }
        public string? URL { get; set; }
    }
}