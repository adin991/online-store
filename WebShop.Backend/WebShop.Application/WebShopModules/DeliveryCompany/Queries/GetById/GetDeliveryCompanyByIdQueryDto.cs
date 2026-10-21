namespace WebShop.Application.WebShopModules.DeliveryCompany.Queries.GetById;

public sealed class GetDeliveryCompanyByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public List<DeliveryCompany_DeliveryDto> Deliverys { get; init; } = new();
}

public sealed class DeliveryCompany_DeliveryDto
{ 
public required int Id { get; init; }
public required DateTime DateOfOrder { get; init; }
public DateTime? DateOfPickUp { get; init; }
public DateTime? DateOfDelivery { get; init; }
}