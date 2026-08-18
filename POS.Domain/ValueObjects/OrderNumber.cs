using POS.Domain.Common;

namespace POS.Domain.ValueObjects
{
    public sealed class OrderNumber : ValueObject
    {
        public string Value { get; }

        public OrderNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exceptions.DomainException("Order number is required.");

            Value = value;
        }

        public static OrderNumber Generate()
            => new($"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}");

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
