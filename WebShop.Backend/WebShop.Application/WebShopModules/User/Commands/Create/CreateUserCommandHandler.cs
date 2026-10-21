using Microsoft.AspNetCore.Identity;

namespace WebShop.Application.WebShopModules.User.Commands.Create
{
    public class CreateUserCommandHandler(
        IAppDbContext context,
        IPasswordHasher<UserEntity> hasher)
        : IRequestHandler<CreateUserCommand, CreateUserResultDto>
    {
        public async Task<CreateUserResultDto> Handle(CreateUserCommand request, CancellationToken ct)
        {
            var email = request.Email.Trim();
            var userExists = await context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, ct);

            if (userExists)
                throw new WebShopConflictException("A user with this email already exists.");

            var user = new UserEntity
            {
                Name = request.Name.Trim(),
                Surname = request.Surname.Trim(),
                Email = email,
                Gender = request.Gender,
                IsEnabled = true
            };

            user.PasswordHash = hasher.HashPassword(user, request.Password);

            await context.Users.AddAsync(user, ct);

            try
            {
                await context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                // Concurrent request slipped past the AnyAsync check above - the unique
                // index on Email is the actual source of truth.
                throw new WebShopConflictException("A user with this email already exists.");
            }

            return new CreateUserResultDto
            {
                UserId = user.Id
            };
        }
    }
}
