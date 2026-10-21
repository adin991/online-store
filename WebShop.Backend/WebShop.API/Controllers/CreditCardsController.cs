using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebShop.Application.WebShopModules.CreditCard.Commands.Create;
using WebShop.Application.WebShopModules.CreditCard.Commands.Delete;
using WebShop.Application.WebShopModules.CreditCard.Commands.Update;
using WebShop.Application.WebShopModules.CreditCard.Queries.GetById;
using WebShop.Application.WebShopModules.CreditCard.Queries.List;

namespace WebShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardsController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateCreditCardCommand request, CancellationToken ct)
        {
            var id = await sender.Send(request, ct);
            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCreditCardCommand request, CancellationToken ct)
        {
            request.Id = id;
            await sender.Send(request, ct);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteCreditCardCommand { Id = id }, ct);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetCreditCardByIdQueryDto>> GetById(int id, CancellationToken ct)
        {
            var result = await sender.Send(new GetCreditCardByIdQuery { Id = id }, ct);
            return result;
        }

        [HttpGet]
        public async Task<PageResult<ListCreditCardQueryDto>> GetAll([FromQuery] ListCreditCardQuery query, CancellationToken ct)
        {
            var result = await sender.Send(query, ct);
            return result;
        }
    }
}
