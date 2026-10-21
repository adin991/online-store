namespace WebShop.Application.Modules.Catalog.Product.Queries.GetById;

public class GetProductByIdQuery : IRequest<GetProductByIdQueryDto>
{
    public int Id { get; set; }
}