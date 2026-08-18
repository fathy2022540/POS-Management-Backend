using POS.Domain.Enums;

namespace POS.Application.Common.DTOs.POS
{
    public class ProcessPaymentDto
    {
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
    }

    public class PaymentResultDto
    {
        public long PaymentId { get; set; }
        public long OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? GatewayTransactionId { get; set; }
    }
}
