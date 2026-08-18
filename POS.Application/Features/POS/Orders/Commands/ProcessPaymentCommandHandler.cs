using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.DTOs.POS;
using POS.Application.Common.Interfaces;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.Repositories;
using POS.Domain.ValueObjects;

namespace POS.Application.Features.POS.Orders.Commands
{
    public class ProcessPaymentCommand : IRequest<ApiResponses<PaymentResultDto>>
    {
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
    }

    public class ProcessPaymentCommandHandler(
        IPosUnitOfWork unitOfWork,
        IPaymentGatewayService paymentGateway,
        IDomainEventDispatcher domainEventDispatcher)
        : IRequestHandler<ProcessPaymentCommand, ApiResponses<PaymentResultDto>>
    {
        public async Task<ApiResponses<PaymentResultDto>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var order = await unitOfWork.Orders.GetByIdWithItemsAsync(request.OrderId, cancellationToken);
                if (order is null)
                {
                    return ApiResponses<PaymentResultDto>.Failure(StatusResult.NotFound, "Order not found.");
                }

                var amount = new Money(request.Amount);
                string? gatewayTransactionId = null;

                if (request.Method is PaymentMethod.Card or PaymentMethod.MobileWallet)
                {
                    var gatewayResult = await paymentGateway.ChargeAsync(
                        new PaymentGatewayRequest(request.Amount, "USD", request.ReferenceNumber),
                        cancellationToken);

                    if (!gatewayResult.IsSuccess)
                    {
                        order.RecordFailedPayment(amount, request.Method, request.ReferenceNumber, gatewayResult.TransactionId);
                        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                        await unitOfWork.CommitTransactionAsync(cancellationToken);

                        return ApiResponses<PaymentResultDto>.Failure(
                            StatusResult.PaymentRequired,
                            gatewayResult.ErrorMessage ?? "Payment gateway declined the transaction.");
                    }

                    gatewayTransactionId = gatewayResult.TransactionId;
                }

                var payment = order.ProcessPayment(amount, request.Method, request.ReferenceNumber, gatewayTransactionId);

                await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await domainEventDispatcher.DispatchEventsAsync(order, cancellationToken);

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var result = new PaymentResultDto
                {
                    PaymentId = payment.Id,
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Amount = payment.Amount,
                    Method = payment.Method.ToString(),
                    Status = payment.Status.ToString(),
                    GatewayTransactionId = payment.GatewayTransactionId
                };

                return ApiResponses<PaymentResultDto>.Success(result, "Payment processed successfully.");
            }
            catch (DomainException ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponses<PaymentResultDto>.Failure(StatusResult.BadRequest, ex.Message);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
