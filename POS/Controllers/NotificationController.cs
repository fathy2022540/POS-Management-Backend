using Microsoft.AspNetCore.Mvc;
using JRM.API.Controllers.Base;
using JRM.Application.Common.DTOs;
using JRM.Application.Features.NotificationServices.Commands;
using JRM.Application.Features.NotificationServices.Queries;

namespace JRM.API.Controllers
{
    public class NotificationController : BaseApiController
    {
        [HttpGet("GetByUserId/{userId}")]
        public async Task<ActionResult<ApiResponses<UserNotificationsResponse>>> GetByUserId([FromRoute] long userId, [FromQuery] int take = 10)
            => BaseResponseHandler(await Mediator.Send(new GetUserNotificationsQuery(userId, take)));

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponses<bool>>> Create([FromBody] CreateNotificationCommand command)
            => BaseResponseHandler(await Mediator.Send(command));

        [HttpPatch("MarkAllAsRead/{userId}")]
        public async Task<ActionResult<ApiResponses<bool>>> MarkAllAsRead([FromRoute] long userId)
            => BaseResponseHandler(await Mediator.Send(new MarkAllNotificationsAsReadCommand(userId)));

        [HttpPatch("MarkAsRead/{notificationId}/User/{userId}")]
        public async Task<ActionResult<ApiResponses<bool>>> MarkAsRead([FromRoute] long notificationId, [FromRoute] long userId)
            => BaseResponseHandler(await Mediator.Send(new MarkNotificationAsReadCommand(notificationId, userId)));
    }
}