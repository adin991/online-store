using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.Application.WebShopModules.User.Commands.Create;
using WebShop.Application.WebShopModules.User.Commands.Delete;
using WebShop.Application.WebShopModules.User.Commands.Update;
using WebShop.Application.WebShopModules.User.Queries.GetById;
using WebShop.Application.WebShopModules.User.Queries.GetPublicProfile;
using WebShop.Application.WebShopModules.User.Queries.List;
using WebShop.Application.WebShopModules.User.Queries.Me;

namespace WebShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(ISender sender) : ControllerBase
    {
        // Registration
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<CreateUserResultDto>> CreateUser(CreateUserCommand request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);
            return Ok(result);
        }

        // Self or admin (checked in the handler)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteUserCommand { Id = id }, ct);
            return Ok();
        }

        // Self or admin (checked in the handler)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand request, CancellationToken ct)
        {
            request.Id = id;
            await sender.Send(request, ct);
            return Ok();
        }

        // Profile of the logged-in user
        [HttpGet("me")]
        public async Task<ActionResult<GetUserByIdQueryDto>> GetMe(CancellationToken ct)
        {
            return Ok(await sender.Send(new GetMyProfileQuery(), ct));
        }

        // Full profile - self or admin only (checked in the handler)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserByIdQueryDto>> GetById(int id, CancellationToken ct)
        {
            return Ok(await sender.Send(new GetUserByIdQuery { Id = id }, ct));
        }

        // Minimal public seller profile (no email / private fields)
        [AllowAnonymous]
        [HttpGet("{id:int}/public")]
        public async Task<ActionResult<GetPublicUserProfileQueryDto>> GetPublicProfile(int id, CancellationToken ct)
        {
            return Ok(await sender.Send(new GetPublicUserProfileQuery { Id = id }, ct));
        }

        // Paged + filtered user list - admin only (checked in the handler)
        [HttpGet]
        public async Task<PageResult<ListUserQueryDto>> List([FromQuery] ListUserQuery query, CancellationToken ct)
        {
            return await sender.Send(query, ct);
        }
    }
}
