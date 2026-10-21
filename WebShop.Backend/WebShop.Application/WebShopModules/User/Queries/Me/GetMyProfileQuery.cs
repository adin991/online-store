using WebShop.Application.WebShopModules.User.Queries.GetById;

namespace WebShop.Application.WebShopModules.User.Queries.Me;

/// <summary>Profile of the currently logged-in user (GET api/User/me).</summary>
public sealed class GetMyProfileQuery : IRequest<GetUserByIdQueryDto>
{
}

public sealed class GetMyProfileQueryHandler(ISender sender, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyProfileQuery, GetUserByIdQueryDto>
{
    public Task<GetUserByIdQueryDto> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            throw new WebShopBusinessRuleException("401", "User must be logged in.");

        return sender.Send(new GetUserByIdQuery { Id = currentUser.UserId.Value }, ct);
    }
}
