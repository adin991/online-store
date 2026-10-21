namespace WebShop.Application.WebShopModules.Role.Commands.Update;

public class UpdateRoleCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateRoleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAdmin)
            throw new WebShopBusinessRuleException("403", "Only admins can update roles");

        var role = await db.Role.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (role is null)
            throw new WebShopNotFoundException("Role not found");

        // Check whether the new name already exists (except for the current role)
        var roleExists = await db.Role.AnyAsync(r => r.Name.ToLower() == request.Name.ToLower() && r.Id != request.Id, ct);
        if (roleExists)
            throw new WebShopConflictException("Role with this name already exists");

        role.Name = request.Name;
        role.Description = request.Description;

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}