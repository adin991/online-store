using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Application.WebShopModules.Messages.Commands.Delete;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Messages.Commands.Create
{
    public class CreateMessageCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<CreateMessageCommand, int>
    {
        public async Task<int> Handle(CreateMessageCommand request, CancellationToken ct)
        {
            var receiverExists = await context.Users.AnyAsync(u => u.Id == request.ReceiverId, ct);
            if (!receiverExists)
                throw new WebShopNotFoundException("Receiver with this ID doesn't exist");

            if (currentUser.UserId == request.ReceiverId)
                throw new WebShopBusinessRuleException("400", "You cannot send a message to yourself");

            var message = new Message
            {
                SenderId = (int)currentUser.UserId,
                Sender = null!,
                ReceiverId = request.ReceiverId,
                Receiver = null!,
                Content = request.Content,
                RecivedDate = DateTime.UtcNow,
                IsRead = false
            };

            await context.Messages.AddAsync(message, ct);
            await context.SaveChangesAsync(ct);
            return message.Id;
        }
    }
}
