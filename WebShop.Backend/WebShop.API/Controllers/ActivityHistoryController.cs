using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebShop.Application.WebShopModules.ActivityHistory.Commands.Create;
using WebShop.Application.WebShopModules.ActivityHistory.Commands.Delete;
using WebShop.Application.WebShopModules.ActivityHistory.Commands.Update;
using WebShop.Application.WebShopModules.ActivityHistory.Queries.GetById;
using WebShop.Application.WebShopModules.ActivityHistory.Queries.List;

namespace WebShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityHistoryController(ISender sender) : ControllerBase
    {

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateActivityHistoryCommand request, CancellationToken ct)
        {
            var id = await sender.Send(request, ct);
            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateActivityHistoryCommand request, CancellationToken ct)
        {
            request.Id = id;
            await sender.Send(request, ct);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteActivityHistoryCommand { Id = id }, ct);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetActivityHistoryByIdQueryDto>> GetById(int id, CancellationToken ct)
        {
            var result = await sender.Send(new GetActivityHistoryByIdQuery { Id = id }, ct);
            return result;
        }

        [HttpGet]
        public async Task<PageResult<ListActivityHistoryQueryDto>> GetAll([FromQuery] ListActivityHistoryQuery query, CancellationToken ct)
        {
            var result = await sender.Send(query, ct);
            return result;
        }
    }
}
