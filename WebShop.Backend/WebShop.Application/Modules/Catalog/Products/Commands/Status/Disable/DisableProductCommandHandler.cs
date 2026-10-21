namespace WebShop.Application.Modules.Catalog.Products.Commands.Status.Disable;

public sealed class DisableProductCommandHandler(IAppDbContext context)
    : IRequestHandler<DisableProductCommand, Unit>
{
    public async Task<Unit> Handle(DisableProductCommand request, CancellationToken ct)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (product is null)
        {
            throw new WebShopNotFoundException($"Product (ID={request.Id}) nije pronađena.");
        }

        if (!product.IsEnabled) return Unit.Value; // idempotent

        product.IsEnabled = false;

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}