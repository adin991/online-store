namespace WebShop.Application.WebShopModules.User.Queries.List
{
    /// <summary>User administration list - admin only (it contains e-mail addresses).</summary>
    public class ListUserQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
        : IRequestHandler<ListUserQuery, PageResult<ListUserQueryDto>>
    {
        public async Task<PageResult<ListUserQueryDto>> Handle(ListUserQuery request, CancellationToken ct)
        {
            if (!currentUser.IsAdmin)
                throw new WebShopBusinessRuleException("403", "Only an administrator can list users.");

            var q = context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                q = q.Where(u => u.Name.ToLower().Contains(search)
                              || u.Surname.ToLower().Contains(search)
                              || u.Email.ToLower().Contains(search));
            }

            if (request.IsAdmin is not null)
                q = q.Where(u => u.IsAdmin == request.IsAdmin);

            var ordered = request.SortBy switch
            {
                "email" => q.OrderBy(x => x.Email),
                "newest" => q.OrderByDescending(x => x.CreatedAtUtc),
                _ => q.OrderBy(x => x.Name).ThenBy(x => x.Surname)
            };

            var projectedQuery = ordered.Select(x => new ListUserQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Email = x.Email,
                IsAdmin = x.IsAdmin,
                IsEnabled = x.IsEnabled,
                CreatedAtUtc = x.CreatedAtUtc
            });

            return await PageResult<ListUserQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
        }
    }
}
