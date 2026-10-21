namespace WebShop.Application.WebShopModules.DeliveryCompany.Commands.Delete;


public class DeleteDeliveryCompanyCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteDeliveryCompanyCommand, Unit>
{
    public async Task<Unit> Handle(DeleteDeliveryCompanyCommand request, CancellationToken cancellationToken)
    {
        if (!appCurrentUser.IsAdmin)
            throw new WebShopBusinessRuleException("403", "Only an administrator can delete a delivery company.");

        var category = await context.DeliveryCompanys
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (category is null)
            throw new WebShopNotFoundException("Delivery company was not found.");

        context.DeliveryCompanys.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
