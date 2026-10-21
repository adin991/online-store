namespace WebShop.Application.WebShopModules.Role.Queries.GetById;

public class GetRoleByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetRoleByIdQuery, GetRoleByIdQueryDto>
{
    public async Task<GetRoleByIdQueryDto> Handle(GetRoleByIdQuery request, CancellationToken ct)
    {
        var role = await context.Role
            .AsNoTracking()
            .Where(r => r.Id == request.Id)
            .Select(r => new GetRoleByIdQueryDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            })
            .FirstOrDefaultAsync(ct);

        if (role is null)
            throw new WebShopNotFoundException("Role with this ID doesn't exist");

        return role;
    }
}