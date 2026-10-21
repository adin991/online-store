using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.User.Commands.Delete
{
    public class DeleteUserCommandHandler(IAppDbContext context, IAppCurrentUser currentUser) : IRequestHandler<DeleteUserCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
        {
            
            if(!currentUser.IsAdmin && currentUser.UserId != request.Id)
                throw new WebShopBusinessRuleException("403", "You cannot delete this user");

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, ct);
            
            if (user is null)
                throw new WebShopNotFoundException("The user with this ID doesn't exist");

            context.Users.Remove(user);
            await context.SaveChangesAsync(ct);

            return Unit.Value;
        }   
    }
}
