namespace WebShop.API.Controllers;
using WebShop.Application.WebShopModules.DeliveryCompany.Queries.GetById;
using WebShop.Application.WebShopModules.DeliveryCompany.Queries.List;
using WebShop.Application.WebShopModules.DeliveryCompany.Commands.Create;
using WebShop.Application.WebShopModules.DeliveryCompany.Commands.Delete;
using WebShop.Application.WebShopModules.DeliveryCompany.Commands.Update;

[ApiController]
[Route("[controller]")]
public class DeliveryCompanyController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateProduct(CreateDeliveryCompanyCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateDeliveryCompanyCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.Id = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteDeliveryCompanyCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetDeliveryCompanyByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var dc = await sender.Send(new GetDeliveryCompanyByIdQuery { Id = id }, ct);
        return dc; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<PageResult<ListDeliveryCompanysQueryDto>> List([FromQuery] ListDeliveryCompanysQuery query, CancellationToken ct)
    {
        var dc = await sender.Send(query, ct);
        return dc;
    }

}