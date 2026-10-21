namespace WebShop.Application.WebShopModules.CreditCard.Commands.Delete;

public class DeleteCreditCardCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteCreditCardCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCreditCardCommand request, CancellationToken ct)
    {
        var creditCard = await db.CreditCards.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (creditCard is null)
            throw new WebShopNotFoundException("Credit card not found");

        if (!currentUser.IsAdmin && currentUser.UserId != creditCard.UserId)
            throw new WebShopBusinessRuleException("403", "You cannot delete this credit card");

        db.CreditCards.Remove(creditCard);
        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}