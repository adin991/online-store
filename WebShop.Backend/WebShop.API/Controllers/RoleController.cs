using WebShop.Application.WebShopModules.Role.Commands.Create;
using WebShop.Application.WebShopModules.Role.Commands.Delete;
using WebShop.Application.WebShopModules.Role.Commands.Update;
using WebShop.Application.WebShopModules.Role.Queries.GetById;
using WebShop.Application.WebShopModules.Role.Queries.List;

namespace WebShop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateRoleCommand request, CancellationToken ct)
    {
        var id = await sender.Send(request, ct);
        return Ok(new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleCommand request, CancellationToken ct)
    {
        request.Id = id;
        await sender.Send(request, ct);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteRoleCommand { Id = id }, ct);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetRoleByIdQueryDto>> GetById(int id, CancellationToken ct)
    {
        var result = await sender.Send(new GetRoleByIdQuery { Id = id }, ct);
        return result;
    }

    [HttpGet]
    public async Task<PageResult<ListRoleQueryDto>> GetAll([FromQuery] ListRoleQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}