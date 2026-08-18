using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.ParamterConfigs.Commands;
using POS.Application.Features.ParamterConfigs.Queries;


namespace POS.API.Controllers
{

    public class ParameterConfigController : BaseApiController
    {
        [HttpGet]
        [AllowAnonymous] // Parameters are often read by clients
        [ProducesResponseType(typeof(ApiResponses<PaginationResponse<ParamterConfigListRespone>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<PaginationResponse<ParamterConfigListRespone>>>> GetAll([FromQuery] GetallParamterConfigsQyery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpPost("Create")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateParamterConfigCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut("Update")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponses<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponses<bool>>> Update(Guid id, [FromBody] UpdateParamterConfigCommand command)
        {
            return BaseResponseHandler(await Mediator.Send(command));
        }
    }
}
