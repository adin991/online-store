namespace WebShop.Application.WebShopModules.User.Queries.GetById
{
    /// <summary>
    /// Full (private) profile of a user - only the user themselves or an admin may read it.
    /// For the public seller card use GetPublicUserProfileQuery.
    /// </summary>
    public class GetUserByIdQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
        : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryDto>
    {
        public async Task<GetUserByIdQueryDto> Handle(GetUserByIdQuery request, CancellationToken ct)
        {
            if (!currentUser.IsAuthenticated)
                throw new WebShopBusinessRuleException("401", "User must be logged in.");

            if (!currentUser.IsAdmin && currentUser.UserId != request.Id)
                throw new WebShopBusinessRuleException("403", "You are not allowed to view this user.");

            var user = await context.Users
                .AsNoTracking()
                .Where(u => u.Id == request.Id)
                .Select(u => new GetUserByIdQueryDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                    Pfp = u.Pfp,
                    Gender = u.Gender,
                    IsAdmin = u.IsAdmin
                }).FirstOrDefaultAsync(ct);

            if (user is null)
                throw new WebShopNotFoundException("The user with this ID doesn't exist");

            return user;
        }
    }
}
