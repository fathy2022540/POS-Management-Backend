using Microsoft.AspNetCore.Mvc;
using JRM.API.Controllers.Base;
using JRM.Application.Common.DTOs;
using JRM.Application.Features.WorkflowActionServices.Commands.Create;
using JRM.Application.Features.WorkflowActionServices.Commands.Delete;
using JRM.Application.Features.WorkflowActionServices.Commands.Update;
using JRM.Application.Features.WorkflowActionServices.Queries;
namespace JRM.API.Controllers
{
    public class WorkflowActionController : BaseApiController
    {
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponses<PaginationResponse<WorkflowActionListResponse>>>> GetAll([FromQuery] GetAllWorkflowActionsQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponses<WorkflowActionDetailsResponse>>> GetById([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new GetWorkflowActionByIdQuery { Id = id }));

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponses<long>>> Create([FromBody] CreateWorkflowActionCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("Update")]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateWorkflowActionCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResponses<bool>>> Delete([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteWorkflowActionCommand(id)));
    }
}
