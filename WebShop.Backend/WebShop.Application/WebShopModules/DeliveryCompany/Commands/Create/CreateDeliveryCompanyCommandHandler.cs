using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.DeliveryCompany.Commands.Create;

public class CreateDeliveryCompanyCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateDeliveryCompanyCommand, int>
{
    public async Task<int> Handle(CreateDeliveryCompanyCommand request, CancellationToken cancellationToken)
    {
        var normalized = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ValidationException("Name is required.");

        // Check if a company with the same name already exists.
        bool exists = await context.DeliveryCompanys
            .AnyAsync(x => x.Name == normalized, cancellationToken);

        if (exists)
        {
            throw new WebShopConflictException("Company already exists with that name.");
        }

        var dc = new Domain.Entities.WebShopEntities.DeliveryCompany
        {
            Name = request.Name!.Trim(),
        };

        context.DeliveryCompanys.Add(dc);
        await context.SaveChangesAsync(cancellationToken);

        return dc.Id;
    }
}