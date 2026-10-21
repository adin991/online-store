namespace WebShop.Application.Modules.Catalog.Product.Queries.GetById;

public class GetProductByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetProductByIdQuery, GetProductByIdQueryDto>
{
    public async Task<GetProductByIdQueryDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Where(c => c.Id == request.Id)
            .Select(x => new GetProductByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                IsEnabled = x.IsEnabled,
                Description = x.Description,
                Price = x.Price,
                StockQuantity = x.StockQuantity,
                CategoryName = x.Category.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            throw new WebShopNotFoundException($"Product with Id {request.Id} not found.");
        }

        return product;
    }
}