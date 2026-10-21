namespace WebShop.Application.WebShopModules.DeliveryCompany.Queries.GetById;

public class GetProductByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetDeliveryCompanyByIdQuery, GetDeliveryCompanyByIdQueryDto>
{
    public async Task<GetDeliveryCompanyByIdQueryDto> Handle(GetDeliveryCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.DeliveryCompanys
            .Where(c => c.Id == request.Id)
            .Select(x => new GetDeliveryCompanyByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Deliverys=x.Deliveries.Select(d => new DeliveryCompany_DeliveryDto
                {
                    Id = d.Id,
                    DateOfOrder = d.DateOfOrder,
                    DateOfPickUp = d.DateOfPickUp,
                    DateOfDelivery = d.DateOfDelivery
                }).ToList()

            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            throw new WebShopNotFoundException($"Delivery Company with Id {request.Id} not found.");
        }

        return product;
    }
}