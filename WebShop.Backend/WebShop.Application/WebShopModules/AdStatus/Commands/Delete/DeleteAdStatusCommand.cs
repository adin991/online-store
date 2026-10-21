namespace WebShop.Application.Modules.Catalog.AdStatus.Commands.Delete;

public class DeleteAdStatusCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
