using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Common.DTOs.POS;
using POS.Application.Features.POS.Orders.Commands;
using POS.Application.Features.POS.Orders.Queries;
using POS.Domain.Enums;

namespace POS.API.Controllers
{
    public class OrderController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponses<PaginationResponse<OrderDto>>>> GetAll([FromQuery] GetOrdersQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponses<OrderDto>>> GetById([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new GetOrderByIdQuery(id)));

        [HttpGet("daily-sales")]
        public async Task<ActionResult<ApiResponses<decimal>>> GetDailySales([FromQuery] DateTime date)
            => BaseResponseHandler(await Mediator.Send(new GetDailySalesQuery(date)));

        [HttpPost]
        public async Task<ActionResult<ApiResponses<CreateOrderResultDto>>> Create([FromBody] CreateOrderCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPost("payment")]
        public async Task<ActionResult<ApiResponses<PaymentResultDto>>> ProcessPayment([FromBody] ProcessPaymentCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("status")]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateStatus([FromBody] UpdateOrderStatusCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("status/{id:long}/cancel")]
        public async Task<ActionResult<ApiResponses<bool>>> Cancel([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new UpdateOrderStatusCommand(id, OrderStatus.Cancelled)));

        [HttpPut("status/{id:long}/refund")]
        public async Task<ActionResult<ApiResponses<bool>>> Refund([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new UpdateOrderStatusCommand(id, OrderStatus.Refunded)));
    }
}
