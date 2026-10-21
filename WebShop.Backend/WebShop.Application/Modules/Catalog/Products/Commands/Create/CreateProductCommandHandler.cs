namespace WebShop.Application.Modules.Catalog.Product.Commands.Create;

public class CreateProductCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var normalized = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ValidationException("Name is required.");

        // Check if a category with the same name already exists.
        bool exists = await context.Products
            .AnyAsync(x => x.Name == normalized, cancellationToken);

        if (exists)
        {
            throw new WebShopConflictException("Product already exists.");
        }

        var category = new ProductEntity
        {
            Name = request.Name!.Trim(),
            Description = request.Description!.Trim(),
            Price = request.Price,
            StockQuantity = 0,//default
            CategoryId=request.CategoryId,
            IsEnabled = true // deault IsEnabled
        };

        context.Products.Add(category);
        await context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}