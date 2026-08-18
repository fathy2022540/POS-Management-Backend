namespace POS.Application.Common.Interfaces
{
    public record PaymentGatewayRequest(decimal Amount, string Currency, string ReferenceNumber);

    public record PaymentGatewayResult(bool IsSuccess, string? TransactionId, string? ErrorMessage);

    /// <summary>
    /// Implemented by Infrastructure to integrate with external payment gateways (card, mobile wallet).
    /// </summary>
    public interface IPaymentGatewayService
    {
        Task<PaymentGatewayResult> ChargeAsync(PaymentGatewayRequest request, CancellationToken cancellationToken = default);
    }
}
