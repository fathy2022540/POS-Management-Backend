using POS.Domain.Enums;

namespace POS.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public long OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public decimal Amount { get; private set; }
        public PaymentMethod Method { get; private set; }
        public PaymentStatus Status { get; private set; }

        public string ReferenceNumber { get; private set; } = string.Empty;
        public string? GatewayTransactionId { get; private set; }

        private Payment()
        {
        }

        internal static Payment CreateSuccessful(
            long orderId,
            decimal amount,
            PaymentMethod method,
            string referenceNumber,
            string? gatewayTransactionId)
        {
            return new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = method,
                Status = PaymentStatus.Completed,
                ReferenceNumber = referenceNumber,
                GatewayTransactionId = gatewayTransactionId
            };
        }

        internal static Payment CreateFailed(
            long orderId,
            decimal amount,
            PaymentMethod method,
            string referenceNumber,
            string? gatewayTransactionId)
        {
            return new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = method,
                Status = PaymentStatus.Failed,
                ReferenceNumber = referenceNumber,
                GatewayTransactionId = gatewayTransactionId
            };
        }
    }
}
