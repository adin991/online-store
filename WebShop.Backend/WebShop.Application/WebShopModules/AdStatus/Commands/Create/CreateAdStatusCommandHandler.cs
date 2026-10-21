using WebShop.Application.WebShopModules.Ad;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.AdStatus.Commands.Create;

public class CreateAdStatusCommandHandler : IRequestHandler<CreateAdStatusCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IAppCurrentUser _currentUser;

    public CreateAdStatusCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateAdStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException("User must be logged in.");

        var ad = await _context.Ads
            .Include(a => a.AdStatuses)
            .FirstOrDefaultAsync(a => a.Id == request.AdId, cancellationToken)
            ?? throw new WebShopNotFoundException($"Ad (ID={request.AdId}) was not found.");

        // An ad can have at most one normal status and at most one Boosted status.
        if (request.Status == AdStatusType.Boosted)
        {
            AdRules.EnsureCanBoost(_currentUser, ad);

            if (ad.AdStatuses.Any(s => s.Status == AdStatusType.Boosted))
                throw new WebShopConflictException("This ad already has a Boosted status.");
        }
        else
        {
            var existing = AdRules.GetPrimaryStatus(ad.AdStatuses);
            if (existing is not null)
                throw new WebShopConflictException($"This ad already has the status '{existing.Status}'.");

            var isSold = await AdRules.IsSoldAsync(_context, ad.Id, cancellationToken);
            AdRules.EnsureCanChangePrimaryStatus(_currentUser, ad, null, request.Status, isSold);
        }

        var adStatus = new Domain.Entities.WebShopEntities.AdStatus
        {
            AdId = request.AdId,
            Status = request.Status,
        };

        _context.AdStatuss.Add(adStatus);
        await _context.SaveChangesAsync(cancellationToken);

        return adStatus.Id;
    }
}
