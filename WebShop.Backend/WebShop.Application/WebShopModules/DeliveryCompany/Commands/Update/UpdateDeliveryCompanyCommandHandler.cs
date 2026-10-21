
namespace WebShop.Application.WebShopModules.DeliveryCompany.Commands.Update;


public sealed class UpdateDeliveryCompanyCommandHandler(IAppDbContext context)
            : IRequestHandler<UpdateDeliveryCompanyCommand, Unit>
{
    public async Task<Unit> Handle(UpdateDeliveryCompanyCommand request, CancellationToken ct)
    {
        var entity = await context.DeliveryCompanys
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
            throw new WebShopNotFoundException($"Delivery company (ID={request.Id}) was not found.");

        // Check for duplicate name (case-insensitive, except for the same ID)
        var exists = await context.DeliveryCompanys
            .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == request.Name.ToLower(), ct);

        if (exists)
        {
            throw new WebShopConflictException("Delivery Company already exists.");
        }

        entity.Name = request.Name.Trim();
        entity.Deliveries=request.Deliveries;
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}