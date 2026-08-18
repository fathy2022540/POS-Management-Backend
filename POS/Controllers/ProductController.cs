using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Application.Common.DTOs;
using POS.Application.Features.Products.Commands;
using Pos.Application.Features.Products.Commands;
using Pos.Application.Features.Products.Queries;

namespace POS.API.Controllers
{
    public class ProductController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponses<PaginationResponse<ProductDto>>>> GetAll([FromQuery] GetAllProductQuery query)
            => BaseResponseHandler(await Mediator.Send(query));

        [HttpPost]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateProductCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPut]
        public async Task<ActionResult<ApiResponses<bool>>> Update([FromBody] UpdateProductCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponses<bool>>> Delete([FromRoute] long id)
            => BaseResponseHandler(await Mediator.Send(new DeleteProductCommand(id)));
    }
}
