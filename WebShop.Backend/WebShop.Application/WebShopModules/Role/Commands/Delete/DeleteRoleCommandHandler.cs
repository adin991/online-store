namespace WebShop.Application.WebShopModules.Role.Commands.Delete;

public class DeleteRoleCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteRoleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAdmin)
            throw new WebShopBusinessRuleException("403", "Only admins can delete roles");

        var role = await db.Role.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (role is null)
            throw new WebShopNotFoundException("Role not found");

        // Check whether the role is assigned to any user
        var usersWithRole = await db.Users.AnyAsync(u => u.RoleId == request.Id, ct);
        if (usersWithRole)
            throw new WebShopBusinessRuleException("409", "Cannot delete role that has assigned users");

        db.Role.Remove(role);
        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}