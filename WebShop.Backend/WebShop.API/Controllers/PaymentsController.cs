using Microsoft.AspNetCore.Mvc;
using WebShop.Application.WebShopModules.Payment.Commands.ConfirmPayment;
using WebShop.Application.WebShopModules.Payment.Commands.CreateCartPaymentIntent;
using WebShop.Application.WebShopModules.Payment.Commands.CreatePaymentIntent;

namespace WebShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(ISender sender) : ControllerBase
    {
        [HttpPost("create-intent")]
        public async Task<ActionResult<CreatePaymentIntentResultDto>> CreateIntent(
            [FromBody] CreatePaymentIntentCommand request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);
            return Ok(result);
        }

        [HttpPost("create-cart-intent")]
        public async Task<ActionResult<CreateCartPaymentIntentResultDto>> CreateCartIntent(
            [FromBody] CreateCartPaymentIntentCommand request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);
            return Ok(result);
        }

        [HttpPost("confirm")]
        public async Task<ActionResult<ConfirmPaymentResultDto>> Confirm(
            [FromBody] ConfirmPaymentCommand request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);
            return Ok(result);
        }
    }
}
