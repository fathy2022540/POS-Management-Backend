using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.Inventory.Commands;
using POS.Application.Features.Inventory.Queries;
using POS.Application.Features.POS.Inventory.Commands;

namespace POS.API.Controllers
{
    public class InventoryController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponses<PaginationResponse<InventoryLevelDto>>>> GetLevels([FromQuery] GetInventoryLevelsQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpPost("adjust")]
        public async Task<ActionResult<ApiResponses<bool>>> AdjustStock([FromBody] AdjustStockCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPost("transfer")]
        public async Task<ActionResult<ApiResponses<bool>>> Transfer([FromBody] TransferInventoryCommand command)
            => BaseResponseHandler(await Mediator.Send(command));
    }
}
