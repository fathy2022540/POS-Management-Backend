using POS.Domain.Common;
using POS.Domain.Enums;
using POS.Domain.Events;
using POS.Domain.Exceptions;
using POS.Domain.ValueObjects;

namespace POS.Domain.Entities
{
    /// <summary>
    /// Order aggregate root. Owns order lines and payments.
    /// </summary>
    public class Order : AggregateRoot
    {
        public string OrderNumber { get; private set; } = string.Empty;
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; }

        public ICollection<OrderItem> OrderItems { get; private set; } = [];
        public ICollection<Payment> Payments { get; private set; } = [];
        public ICollection<StockTransaction> StockTransactions { get; private set; } = [];

        private Order()
        {
        }

        public static Order Create(OrderNumber orderNumber, IEnumerable<(long ProductId, int Quantity, Money UnitPrice)> lines)
        {
            var order = new Order
            {
                OrderNumber = orderNumber.Value,
                Status = OrderStatus.Pending,
                TotalAmount = 0
            };

            foreach (var line in lines)
            {
                order.AddLineItem(line.ProductId, line.Quantity, line.UnitPrice);
            }

            return order;
        }

        public void NotifyCreated()
        {
            AddDomainEvent(new OrderCreatedDomainEvent(Id, OrderNumber));
        }

        public void AddLineItem(long productId, int quantity, Money unitPrice)
        {
            EnsureModifiable();

            if (productId <= 0)
            {
                throw new OrderDomainException("Product id is required.");
            }

            if (quantity <= 0)
            {
                throw new OrderDomainException("Quantity must be greater than zero.");
            }

            var lineTotal = unitPrice * quantity;

            OrderItems.Add(new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice.Amount
            });

            TotalAmount += lineTotal.Amount;
        }

        public Payment ProcessPayment(Money amount, PaymentMethod method, string referenceNumber, string? gatewayTransactionId)
        {
            EnsurePending();

            if (amount.Amount != TotalAmount)
            {
                throw new OrderDomainException($"Payment amount {amount.Amount} does not match order total {TotalAmount}.");
            }

            var payment = Payment.CreateSuccessful(Id, amount.Amount, method, referenceNumber, gatewayTransactionId);
            Payments.Add(payment);
            Status = OrderStatus.Paid;

            AddDomainEvent(new OrderPaidDomainEvent(
                Id,
                OrderNumber,
                OrderItems.Select(i => new OrderLineSnapshot(i.ProductId, i.Quantity)).ToList()));

            return payment;
        }

        public Payment RecordFailedPayment(Money amount, PaymentMethod method, string referenceNumber, string? gatewayTransactionId)
        {
            EnsurePending();

            var payment = Payment.CreateFailed(Id, amount.Amount, method, referenceNumber, gatewayTransactionId);
            Payments.Add(payment);
            return payment;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new OrderDomainException($"Only pending orders can be cancelled. Current status: {Status}.");
            }

            Status = OrderStatus.Cancelled;
        }

        public void Refund()
        {
            if (Status != OrderStatus.Paid)
            {
                throw new OrderDomainException($"Only paid orders can be refunded. Current status: {Status}.");
            }

            Status = OrderStatus.Refunded;
        }

        private void EnsurePending()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new OrderDomainException($"Order must be pending. Current status: {Status}.");
            }
        }

        private void EnsureModifiable()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new OrderDomainException($"Order cannot be modified when status is {Status}.");
            }
        }
    }
}
