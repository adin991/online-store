namespace WebShop.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using WebShop.Application.WebShopModules.Ad.Queries.List;
using WebShop.Application.WebShopModules.Ad.Queries.GetById;
using WebShop.Application.WebShopModules.Ad.Commands.Create;
using WebShop.Application.WebShopModules.Ad.Commands.Delete;
using WebShop.Application.WebShopModules.Ad.Commands.Update;
using WebShop.Application.WebShopModules.Ad.Commands.Status;
using WebShop.Domain.Entities.WebShopEntities;

[ApiController]
[Route("[controller]")]
public class AdsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateProduct(CreateAdCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateAdCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.Id = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteAdCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<GetAdByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var result = await sender.Send(new GetAdByIdQuery { Id = id }, ct);
        return result; // if NotFoundException -> 404 via middleware
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<PageResult<ListAdsQueryDto>> List([FromQuery] ListAdsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }


    // Owner or admin only (checked in the handler). Changes the primary status explicitly;
    // statusType=Boosted adds the Boosted flag instead.
    [HttpPut("{id:int}/changeAdStatus")]
    public async Task ChangeStatus(int id, AdStatusType statusType, CancellationToken ct)
    {
        await sender.Send(new UpdateStatusThruAdCommand { Id = id, AdStatusType = statusType }, ct);
        // no return -> 204 No Content
    }
}