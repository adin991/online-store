using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad.Commands.Create;

public class CreateAdCommandHandler : IRequestHandler<CreateAdCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IAppCurrentUser _currentUser;

    public CreateAdCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateAdCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException("User must be logged in.");

        // Field rules (required, lengths, price range) are enforced by CreateAdCommandValidator.
        var category = await _context.ProductCategories.FindAsync(new object[] { request.CategoryId }, cancellationToken)
            ?? throw new ValidationException("Invalid category.");

        var ad = new Domain.Entities.WebShopEntities.Ad
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            UserId = _currentUser.UserId!.Value,
            Images = request.Images ?? new List<string>(),
            Price = request.Price,
            Location = request.Location?.Trim(),
            Colors = AdOptions.Normalize(request.Colors),
            Sizes = AdOptions.Normalize(request.Sizes),
            CategoryEntity = category,

            AdStatuses = new List<Domain.Entities.WebShopEntities.AdStatus>
            {
                new Domain.Entities.WebShopEntities.AdStatus
                {
                    Status = AdStatusType.Live // default status
                }
            }
        };

        _context.Ads.Add(ad);
        await _context.SaveChangesAsync(cancellationToken);

        return ad.Id;
    }
}
