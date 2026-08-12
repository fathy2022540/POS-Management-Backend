using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.LookupServices.Commands;
using POS.Application.Features.LookupServices.Queries;

namespace POS.API.Controllers
{
    public class LookupController : BaseApiController
    {
        // ==========================
        // Lookups Endpoints
        // ==========================

        [HttpGet("GetLookupsData")]
        public async Task<ActionResult<ApiResponses<PaginationResponse<LookupDto>>>> GetAll([FromQuery] GetAllLookupQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponses<bool>>> Add([FromBody] CreateLookupCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("Update")]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateLookupCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResponses<bool>>> Delete([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteLookupCommand(id)));

        // ==========================
        // Lookup Items Endpoints
        // ==========================

        [HttpGet("GetAllItemsListByLookupId")]
        public async Task<ActionResult<ApiResponses<PaginationResponse<LookupItemDto>>>> GetAllItemsListByLookupId([FromQuery] GetAllLookupItemsListByLookupIdQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpGet("GetAllItemsByLookupId")]
        public async Task<ActionResult<ApiResponses<IEnumerable<LookupItemDto>>>> GetAllItemsByLookupId([FromQuery] GetAllLookupItemsByLookupIdQuery query)
    => BaseResponseHandler(await Mediator.Send(query));

        [HttpPost("CreateItem")]
        public async Task<ActionResult<ApiResponses<bool>>> AddItem([FromBody] CreateLookupItemsCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("UpdateItem")]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateItem([FromBody] UpdateLookupItemsCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpDelete("DeleteItem/{id}")]
        public async Task<ActionResult<ApiResponses<bool>>> DeleteItem([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteLookupItemsCommand(id)));
    }
}