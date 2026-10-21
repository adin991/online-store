namespace WebShop.Application.WebShopModules.DeliveryCompany.Commands.Delete;

public class DeleteDeliveryCompanyCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
