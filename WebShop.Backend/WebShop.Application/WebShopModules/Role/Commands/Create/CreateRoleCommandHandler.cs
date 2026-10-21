namespace WebShop.Application.WebShopModules.Role.Commands.Create;

public class CreateRoleCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<CreateRoleCommand, int>
{
    public async Task<int> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAdmin)
            throw new WebShopBusinessRuleException("403", "Only admins can create roles");

        // Check whether a role with this name already exists
        var roleExists = await db.Role.AnyAsync(r => r.Name.ToLower() == request.Name.ToLower(), ct);
        if (roleExists)
            throw new WebShopConflictException("Role with this name already exists");

        var role = new Domain.Entities.WebShopEntities.Role
        {
            Name = request.Name,
            Description = request.Description
        };

        await db.Role.AddAsync(role, ct);
        await db.SaveChangesAsync(ct);
        return role.Id;
    }
}