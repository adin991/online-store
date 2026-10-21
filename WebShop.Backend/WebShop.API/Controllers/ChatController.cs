using Microsoft.AspNetCore.Http.Features;
using WebShop.Application.Modules.Chat.Commands.SendMessage;

namespace WebShop.API.Controllers;

[ApiController]
[Route("api/chat")]
public sealed class ChatController(IMediator mediator) : ControllerBase
{
    [HttpPost("stream")]
    [AllowAnonymous]
    public async Task Stream([FromBody] SendMessageCommand command, CancellationToken ct)
    {
        Response.ContentType = "text/plain; charset=utf-8";
        Response.Headers.CacheControl = "no-cache";
        HttpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();

        await foreach (var chunk in mediator.CreateStream(command, ct))
        {
            await Response.WriteAsync(chunk, ct);
            await Response.Body.FlushAsync(ct);
        }
    }
}
