using Microsoft.AspNetCore.Mvc;
using TendersManagament.API.Controllers.Base;
using TendersManagament.Application.CommandServices.VendorProductPricesServices;
using TendersManagament.Application.Common.DTOs;
using TendersManagament.Application.QueriesServices.VendorProductPrices.GetAll;
using TendersManagament.Application.QueriesServices.VendorProductPrices.GetVendorPriceHistory;

namespace TendersManagament.API.Controllers
{

    public class VendorProductPriceController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateVendorPriceCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponses<bool>>> Add([FromBody] CreateVendorPriceCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        //[HttpPatch("UpdateStatus")]
        //public async Task<ActionResult<Result<bool>>> UpdateStatus([FromBody] UpdatVendorProductPriceStatusCommand command)
        //   => BaseResponseHandler(await Mediator.Send(command));

        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponses<PaginationResponse<VendorProductPriceListResponse>>>> GetAll([FromQuery] GetAllVendorsProductPricesQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpGet("GetHistory")]
        public async Task<ActionResult<ApiResponses<PaginationResponse<VendorPriceHistoryResponse>>>> GetHistory([FromQuery] GetVendorPriceHistoryQuery query)
            => BaseResponseHandler(await Mediator.Send(query));


    }
}
