namespace WebShop.Application.WebShopModules.DeliveryCompany.Commands.Create;

public class CreateDeliveryCompanyCommand : IRequest<int>
{
    public required string Name { get; set; }
}