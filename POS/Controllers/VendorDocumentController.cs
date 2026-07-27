using Microsoft.AspNetCore.Mvc;
using JRM.API.Controllers.Base;
using JRM.Application.Common.DTOs;
using JRM.Application.Features.VendorServices.Commands;


namespace JRM.API.Controllers
{
    
    public class VendorDocumentController : BaseApiController
    {
        //[HttpGet("Getall")]
        //public async Task<ActionResult<ApiResponses<PaginationResponse<VendorDocumentsListRespone>>>> GetAll([FromQuery] GetAllVendorDocumentQuery query)
        //    =>  BaseResponseHandler(await Mediator.Send(query));

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateVendorDocumentsCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("Update")]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateVendorDocumentCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpDelete("Delete")]
        public async Task<ActionResult<ApiResponses<bool>>> Delete(DeleteVendorDocumentsCommand command)
            => BaseResponseHandler(await Mediator.Send(command));
    }
}
