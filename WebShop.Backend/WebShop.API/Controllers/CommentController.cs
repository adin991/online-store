using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebShop.Application.WebShopModules.Comment.Commands.Update;
using WebShop.Application.WebShopModules.Comment.Queries.GetById;
using WebShop.Application.WebShopModules.Comment.Queries.List;
using WebShop.Application.WebShopModules.Comments.Commands.Create;
using WebShop.Application.WebShopModules.Comments.Commands.Delete;

namespace WebShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateCommentCommand request, CancellationToken ct)
        {
            var id = await sender.Send(request, ct);
            return Ok(new { id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteCommentCommand { Id = id }, ct);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentCommand request, CancellationToken ct)
        {
            request.Id = id;
            await sender.Send(request, ct);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCommentByIdQueryDto>> GetById(int id, CancellationToken ct)
        {
            var result = await sender.Send(new GetCommentByIdQuery { Id = id }, ct);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PageResult<ListCommentsQueryDto>>> List([FromQuery] ListCommentsQuery request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);
            return Ok(result);
        }
    }
}
