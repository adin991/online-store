using Azure.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebShop.Application.WebShopModules.Notifications.Commands.Create;
using WebShop.Application.WebShopModules.Notifications.Commands.Delete;
using WebShop.Application.WebShopModules.Notifications.Commands.Update;
using WebShop.Application.WebShopModules.Notifications.Queries.GetById;
using WebShop.Application.WebShopModules.Notifications.Queries.List;

namespace WebShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> CreateNotification([FromBody] CreateNotificationsCommand request, CancellationToken ct)
        {
            var id = await sender.Send(request, ct);
            return Ok(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteNotificationsCommand { Id = id }, ct);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] UpdateNotificationsCommand request, CancellationToken ct)
        {
            request.Id = id;
            await sender.Send(request, ct);
            return Ok();
        }

        // query
        [HttpGet("{id}")]
        public async Task<ActionResult<GetNotificationsByIdQueryDto>> GetNotification(int id, CancellationToken ct)
        {
            var notification = await sender.Send(new GetNotificationsByIdQuery { Id = id }, ct);
            return Ok(notification);
        }

        //paging
        [HttpGet]
        public async Task<ActionResult<PageResult<ListNotificationsQueryDto>>> GetNotifications([FromQuery] ListNotificationsQuery request, CancellationToken ct)
        {
            var notifications = await sender.Send(request, ct);
            return Ok(notifications);
        }
    }
}
