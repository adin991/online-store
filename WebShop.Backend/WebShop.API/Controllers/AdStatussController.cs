namespace WebShop.API.Controllers;

using WebShop.Application.Modules.Catalog.AdStatus.Commands.Delete;
using WebShop.Application.WebShopModules.Ad.Commands.Status;
using WebShop.Application.WebShopModules.AdStatus.Commands.Create;
using WebShop.Application.WebShopModules.AdStatus.Commands.Update;
using WebShop.Application.WebShopModules.AdStatus.Queries.GetById;
using WebShop.Application.WebShopModules.AdStatus.Queries.List;
using WebShop.Application.WebShopModules.AdStatuss.Queries.List;
using WebShop.Domain.Entities.WebShopEntities;

[ApiController]
[Route("[controller]")]
public class AdStatussController(ISender sender) : ControllerBase
{
    // Owner or admin only (checked in the handler)
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateAdStatusCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    // Owner or admin only (checked in the handler)
    [HttpPut("{id:int}")]
    public async Task Update(int id, AdStatusType statusType, CancellationToken ct)
    {
        await sender.Send(new UpdateAdStatusCommand { Id = id, Status = statusType }, ct);
    }

    // Admin only (checked in the handler)
    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteAdStatusCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetAdStatusByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var result = await sender.Send(new GetAdStatusByIdQuery { Id = id}, ct);
        return result; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<PageResult<ListAdStatussQueryDto>> List([FromQuery] ListAdStautssQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

}