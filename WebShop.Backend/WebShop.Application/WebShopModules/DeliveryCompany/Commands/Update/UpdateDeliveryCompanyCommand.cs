using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.DeliveryCompany.Commands.Update;

public sealed class UpdateDeliveryCompanyCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required List<Delivery> Deliveries { get; set; } 
}
// TODO: update deliveries by id only and add an Add method