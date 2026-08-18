using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.Stores.Commands;
using POS.Application.Features.Stores.Queries;

namespace POS.API.Controllers
{
    public class StoreController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponses<PaginationResponse<StoreDto>>>> GetAll([FromQuery] GetStoresQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpPost]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateStoreCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateStoreCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponses<bool>>> Delete([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteStoreCommand(id)));
    }
}
