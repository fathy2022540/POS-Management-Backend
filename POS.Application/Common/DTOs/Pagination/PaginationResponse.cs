namespace JRM.Application.Common.DTOs
{
    public class PaginationResponse<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; } = [];

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
