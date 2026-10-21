namespace WebShop.Application.Modules.Catalog.Products.Commands.Status.Enable;

public sealed class EnableProductCommandHandler(IAppDbContext context)
    : IRequestHandler<EnableProductCommand, Unit>
{
    public async Task<Unit> Handle(EnableProductCommand request, CancellationToken ct)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (product is null)
            throw new WebShopNotFoundException($"Product (ID={request.Id}) nije pronađena.");

        if (!product.IsEnabled)
        {
            product.IsEnabled = true;
            await context.SaveChangesAsync(ct);
        }

        // If already enabled — nothing changes, idempotent
        return Unit.Value;
    }
}