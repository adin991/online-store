
using WebShop.Application.WebShopModules.AdStatus.Queries.List;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.AdStatus.Queries.GetById;
public class GetAdStatusByIdQueryHandler(IAppDbContext context) 
    : IRequestHandler<GetAdStatusByIdQuery, GetAdStatusByIdQueryDto>{
    public async Task<GetAdStatusByIdQueryDto> Handle(GetAdStatusByIdQuery request, CancellationToken cancellationToken)
    {

        var ad = await context.AdStatuss
            .Where(c => c.Id == request.Id)
            .Select(x => new GetAdStatusByIdQueryDto
            {
                Id = x.Id,
                AdStatus = x.Status,
                StatusName = x.Status.ToString(),
                AdId = x.Ad.Id,
                
                AdDto = new GetAdStatusByIdAdDto
                {
                    Title = x.Ad.Title,
                }

            })
            .FirstOrDefaultAsync(cancellationToken);

        if (ad == null)
        {
            throw new WebShopNotFoundException($"Ad Status with Id {request.Id} not found.");
        }

        return ad;
    }
}