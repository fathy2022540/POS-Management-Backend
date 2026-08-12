

using POS.Domain.Enums;

namespace POS.Application.Common.DTOs
{
    public class CreateProductDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool TrackInventory { get; set; }
        public bool IsComposite { get; set; }
    }
    public class UpdateProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
    public class ProductDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool TrackInventory { get; set; }
    }
}
