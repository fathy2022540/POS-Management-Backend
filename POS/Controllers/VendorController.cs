using Microsoft.AspNetCore.Mvc;
using JRM.API.Controllers.Base;
using JRM.Application.Common.DTOs;
using JRM.Application.Features.ProductServices.Queries;
using JRM.Application.Features.VendorServices.Commands;
using JRM.Application.Features.VendorServices.Queries;

namespace JRM.API.Controllers
{
    public class VendorController : BaseApiController
    {
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponses<PaginationResponse<VendorListResponse>>>> GetAll([FromQuery] GetAllVendorsQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        // FIXED: Changed from query object to clean route parameters
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponses<JourneyDto>>> GetById([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new GetVendorByIdQuery { Id = id }));

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateVendorCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("Update")]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateVendorCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPatch("UpdateStatus")]
        public async Task<ActionResult<ApiResponses<bool>>> UpdateStatus([FromBody] UpdateStatusCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        // FIXED: Removed [FromBody]. Passed Id through the route or query string instead.
        [HttpDelete("Delete")]
        public async Task<ActionResult<ApiResponses<bool>>> Delete([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new Application.Features.VendorServices.Commands.DeleteVendorCommand(id)));


    }
}