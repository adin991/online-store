namespace WebShop.Application.Modules.Catalog.Product.Commands.Delete;

public class DeleteProductCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
