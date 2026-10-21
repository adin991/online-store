namespace WebShop.Application.WebShopModules.CreditCard.Queries.List;

public class ListCreditCardQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListCreditCardQuery, PageResult<ListCreditCardQueryDto>>
{
    public async Task<PageResult<ListCreditCardQueryDto>> Handle(ListCreditCardQuery request, CancellationToken ct)
    {
        var q = context.CreditCards.AsNoTracking();

        // A user can only see their own cards unless they are an admin
        if (!currentUser.IsAdmin)
            q = q.Where(c => c.UserId == currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(request.CardTypeFilter))
            q = q.Where(c => c.CardType.ToLower().Contains(request.CardTypeFilter.ToLower()));

        if (request.UserId is not null)
            q = q.Where(c => c.UserId == request.UserId);

        var projectedQuery = q.OrderByDescending(c => c.CreatedAtUtc)
            .Select(c => new ListCreditCardQueryDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Last4 = c.Last4,
                CardType = c.CardType,
                ExpMonth = c.ExpMonth,
                ExpYear = c.ExpYear,
                User = new ListCreditCardQueryUserDto
                {
                    Id = c.User.Id,
                    Name = c.User.Name,
                    Surname = c.User.Surname
                }
            });

        return await PageResult<ListCreditCardQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}