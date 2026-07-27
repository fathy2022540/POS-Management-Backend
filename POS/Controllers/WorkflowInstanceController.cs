using Microsoft.AspNetCore.Mvc;
using JRM.API.Controllers.Base;
using JRM.Application.Common.DTOs;
using JRM.Application.Features.WorkflowInstanceServices.Commands.Create;
using JRM.Application.Features.WorkflowInstanceServices.Commands.Delete;
using JRM.Application.Features.WorkflowInstanceServices.Commands.Update;
using JRM.Application.Features.WorkflowInstanceServices.Quieries;

namespace JRM.API.Controllers
{
    public class WorkflowInstanceController : BaseApiController
    {
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponses<PaginationResponse<WorkflowInstanceListResponse>>>> GetAll([FromQuery] GetAllWorkflowInstancesQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponses<WorkflowInstanceDetailsResponse>>> GetById([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new GetWorkflowInstanceByIdQuery { Id = id }));

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponses<long>>> Create([FromBody] CreateWorkflowInstanceCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("Update")]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateWorkflowInstanceCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResponses<bool>>> Delete([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteWorkflowInstanceCommand(id)));
    }
}
