using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Commands.Delete
{
    public class DeleteMessageCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteMessageCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteMessageCommand request, CancellationToken ct)
        {
            var message = await context.Messages.FirstOrDefaultAsync(m => m.Id == request.Id, ct);
            if (message is null)
                throw new WebShopNotFoundException("Message with this ID doesn't exist");

            if (!currentUser.IsAdmin && currentUser.UserId != message.SenderId)
                throw new WebShopBusinessRuleException("403", "You cannot delete this message");

            context.Messages.Remove(message);
            await context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
