using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebShop.Application.WebShopModules.Messages.Commands.Create;
using WebShop.Application.WebShopModules.Messages.Commands.Delete;
using WebShop.Application.WebShopModules.Messages.Commands.Update;
using WebShop.Application.WebShopModules.Messages.Queries.GetById;
using WebShop.Application.WebShopModules.Messages.Queries.List;

namespace WebShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateMessageCommand request, CancellationToken ct)
        {
            var id = await sender.Send(request, ct);
            return Ok(new { id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteMessageCommand { Id = id }, ct);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMessageCommand request, CancellationToken ct)
        {
            request.Id = id;
            await sender.Send(request, ct);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetMessageByIdQueryDto>> GetById(int id, CancellationToken ct)
        {
            var result = await sender.Send(new GetMessageByIdQuery { Id = id }, ct);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PageResult<ListMessagesQueryDto>>> List([FromQuery] ListMessagesQuery request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);
            return Ok(result);
        }

    }
}
