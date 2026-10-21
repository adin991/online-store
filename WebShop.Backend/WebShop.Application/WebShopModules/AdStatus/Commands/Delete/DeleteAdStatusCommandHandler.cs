

namespace WebShop.Application.Modules.Catalog.AdStatus.Commands.Delete;

public class DeleteAdStatusCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteAdStatusCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAdStatusCommand request, CancellationToken cancellationToken)
    {
        if (!appCurrentUser.IsAdmin)
            throw new WebShopBusinessRuleException("403", "Only an administrator can delete an ad status.");

        var adstatus = await context.AdStatuss
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (adstatus is null)
            throw new WebShopNotFoundException("Ad status was not found.");

        context.AdStatuss.Remove(adstatus);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
