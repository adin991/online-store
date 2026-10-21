namespace WebShop.Application.WebShopModules.CreditCard.Queries.GetById;

public class GetCreditCardByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCreditCardByIdQuery, GetCreditCardByIdQueryDto>
{
    public async Task<GetCreditCardByIdQueryDto> Handle(GetCreditCardByIdQuery request, CancellationToken ct)
    {
        var creditCard = await context.CreditCards
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new GetCreditCardByIdQueryDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Last4 = c.Last4,
                CardType = c.CardType,
                ExpMonth = c.ExpMonth,
                ExpYear = c.ExpYear,
                User = new GetCreditCardByIdUserDto
                {
                    Id = c.User.Id,
                    Name = c.User.Name,
                    Surname = c.User.Surname
                }
            })
            .FirstOrDefaultAsync(ct);

        if (creditCard is null)
            throw new WebShopNotFoundException("Credit card with this ID doesn't exist");

        return creditCard;
    }
}