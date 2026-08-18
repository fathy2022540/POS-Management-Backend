using POS.Domain.Common;

namespace POS.Domain.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }

        public Money(decimal amount)
        {
            if (amount < 0)
                throw new Exceptions.DomainException("Money amount cannot be negative.");

            Amount = amount;
        }

        public static Money Zero => new(0);

        public Money Add(Money other) => new(Amount + other.Amount);

        public static Money operator *(Money money, int quantity) => new(money.Amount * quantity);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
        }

        public override string ToString() => Amount.ToString("F2");
    }
}
