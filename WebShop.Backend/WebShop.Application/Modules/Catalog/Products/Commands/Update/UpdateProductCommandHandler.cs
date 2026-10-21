namespace WebShop.Application.Modules.Catalog.Product.Commands.Update;

public sealed class UpdateProductCommandHandler(IAppDbContext context)
            : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var entity = await context.Products
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
            throw new WebShopNotFoundException($"Product (ID={request.Id}) nije pronađena.");

        // Check for duplicate name (case-insensitive, except for the same ID)
        var exists = await context.Products
            .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == request.Name.ToLower(), ct);

        if (exists)
        {
            throw new WebShopConflictException("Name already exists.");
        }
        //product category doesnt exist
        var productcategory = await context.ProductCategories.Where(x => x.Id == request.CategoryId).FirstOrDefaultAsync(ct);
        if (productcategory == null)
        {
            throw new Exception("Invalid category");
        }
        //product category isnt enabled
        var productcategoryenable = await context.ProductCategories.Where(x => x.Id == request.CategoryId).FirstOrDefaultAsync(ct);
        if (productcategory.IsEnabled==false)
        {
            throw new Exception("Category is dissabled");
        }

        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.StockQuantity = 0;
        entity.Price=request.Price;
        entity.CategoryId = request.CategoryId;
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}