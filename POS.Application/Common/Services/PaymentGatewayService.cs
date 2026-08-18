using POS.Application.Common.Interfaces;

namespace POS.Application.Common.Services
{
    /// <summary>
    /// Default payment gateway stub. Replace registration with an Infrastructure implementation for production gateways.
    /// </summary>
    public class PaymentGatewayService : IPaymentGatewayService
    {
        public Task<PaymentGatewayResult> ChargeAsync(PaymentGatewayRequest request, CancellationToken cancellationToken = default)
        {
            var transactionId = $"TXN-{Guid.NewGuid():N}"[..20];
            return Task.FromResult(new PaymentGatewayResult(true, transactionId, null));
        }
    }
}
