using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebShop.Application.WebShopModules.SearchHistory.Commands.Create;
using WebShop.Application.WebShopModules.SearchHistory.Commands.Delete;
using WebShop.Application.WebShopModules.SearchHistory.Commands.Update;
using WebShop.Application.WebShopModules.SearchHistory.Queries.GetById;
using WebShop.Application.WebShopModules.SearchHistory.Queries.List;

namespace WebShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchHistoryController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateSearchHistoryCommand request, CancellationToken ct)
        {
            var id = await sender.Send(request, ct);
            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSearchHistoryCommand request, CancellationToken ct)
        {
            request.Id = id;
            await sender.Send(request, ct);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteSearchHistoryCommand { Id = id }, ct);
            return Ok();
        }

        // query
        [HttpGet("{Id}")]
        public async Task<ActionResult<GetSearchHistoryByIdQueryDto>> GetById(int Id, CancellationToken ct)
        {
            var result = await sender.Send(new GetSearchHistoryByIdQuery { Id = Id }, ct);
            return result;
        }

        // paging
        [HttpGet]
        public async Task<PageResult<ListSearchHistoryQueryDto>> GetAll([FromQuery] ListSearchHistoryQuery query, CancellationToken ct)
        {
            var result = await sender.Send(query, ct);
            return result;

        }
    }
}
