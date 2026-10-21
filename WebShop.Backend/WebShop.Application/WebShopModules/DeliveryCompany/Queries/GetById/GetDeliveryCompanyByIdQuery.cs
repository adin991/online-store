namespace WebShop.Application.WebShopModules.DeliveryCompany.Queries.GetById;

public class GetDeliveryCompanyByIdQuery : IRequest<GetDeliveryCompanyByIdQueryDto>
{
    public int Id { get; set; }
}