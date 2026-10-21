namespace WebShop.Application.WebShopModules.CreditCard.Commands.Create;

public class CreateCreditCardCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<CreateCreditCardCommand, int>
{
    public async Task<int> Handle(CreateCreditCardCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new WebShopBusinessRuleException("401", "User must be logged in");

        var userExists = await db.Users.AnyAsync(u => u.Id == currentUser.UserId, ct);
        if (!userExists)
            throw new WebShopNotFoundException("User not found");

        // Validacija mjeseca i godine
        if (request.ExpMonth < 1 || request.ExpMonth > 12)
            throw new WebShopBusinessRuleException("400", "Expiration month must be between 1 and 12");

        var currentYear = DateTime.UtcNow.Year;
        if (request.ExpYear < currentYear || request.ExpYear > currentYear + 20)
            throw new WebShopBusinessRuleException("400", "Expiration year is invalid");

        var creditCard = new Domain.Entities.WebShopEntities.CreditCard
        {
            UserId = (int)currentUser.UserId,
            Last4 = request.Last4,
            CardType = request.CardType,
            ExpMonth = request.ExpMonth,
            ExpYear = request.ExpYear
        };

        await db.CreditCards.AddAsync(creditCard, ct);
        await db.SaveChangesAsync(ct);
        return creditCard.Id;
    }
}