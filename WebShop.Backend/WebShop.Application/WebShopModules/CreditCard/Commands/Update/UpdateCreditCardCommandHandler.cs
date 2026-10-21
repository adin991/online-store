namespace WebShop.Application.WebShopModules.CreditCard.Commands.Update;

public class UpdateCreditCardCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateCreditCardCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCreditCardCommand request, CancellationToken ct)
    {
        var creditCard = await db.CreditCards.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (creditCard is null)
            throw new WebShopNotFoundException("Credit card not found");

        if (!currentUser.IsAdmin && currentUser.UserId != creditCard.UserId)
            throw new WebShopBusinessRuleException("403", "You cannot update this credit card");

        // Validacija mjeseca i godine
        if (request.ExpMonth < 1 || request.ExpMonth > 12)
            throw new WebShopBusinessRuleException("400", "Expiration month must be between 1 and 12");

        var currentYear = DateTime.UtcNow.Year;
        if (request.ExpYear < currentYear || request.ExpYear > currentYear + 20)
            throw new WebShopBusinessRuleException("400", "Expiration year is invalid");

        creditCard.Last4 = request.Last4;
        creditCard.CardType = request.CardType;
        creditCard.ExpMonth = request.ExpMonth;
        creditCard.ExpYear = request.ExpYear;

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}