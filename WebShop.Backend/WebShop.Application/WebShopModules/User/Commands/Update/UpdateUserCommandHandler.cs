using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.User.Commands.Update
{
    public class UpdateUserCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateUserCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken ct)
        {
            if (!currentUser.IsAdmin && currentUser.UserId != request.Id)
                throw new WebShopBusinessRuleException("403", "You cannot update this user");

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, ct);

            if (user is null)
                throw new WebShopNotFoundException("The user with this ID doesn't exist");

            // exclude the current user from the uniqueness check
            var email = request.Email.Trim();
            var emailTaken = await context.Users.AnyAsync(u => u.Email == email && u.Id != request.Id, ct);

            if (emailTaken)
            {
                throw new WebShopBusinessRuleException("400", "This email is already in use");
            }
            // apply changes
            user.Name = request.Name.Trim();
            user.Surname = request.Surname.Trim();
            user.Email = email;
            user.Gender = request.Gender;

            await context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
