namespace POS.Application.Common.DTOs
{
    public record LookupDto(long Id, string Name, string NameAR, DateTime CreatedDate);
    public record LookupDetailsDto(
        long Id,
        string Name,
        string NameAR,
        List<LookupItemDto> Items
    );

    public record LookupItemDto(
        long Id,
        long LookupId,
        string LookupName,
        string? LookupNameAr,
        long? LookupItemParentId,
        string NameEn,
        string? NameAR,
        string? Code
    );
}


