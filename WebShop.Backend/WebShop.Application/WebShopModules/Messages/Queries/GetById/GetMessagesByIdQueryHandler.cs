using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Queries.GetById
{
    public class GetMessageByIdQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<GetMessageByIdQuery, GetMessageByIdQueryDto>
    {
        public async Task<GetMessageByIdQueryDto> Handle(GetMessageByIdQuery request, CancellationToken ct)
        {
            var message = await context.Messages
                .AsNoTracking()
                .Where(m => m.Id == request.Id)
                .Select(m => new GetMessageByIdQueryDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    IsRead = m.IsRead,
                    RecivedDate = m.RecivedDate,
                    Sender = new GetMessageByIdUserDto
                    {
                        Id = m.Sender.Id,
                        Name = m.Sender.Name,
                        Surname = m.Sender.Surname
                    },
                    Receiver = new GetMessageByIdUserDto
                    {
                        Id = m.Receiver.Id,
                        Name = m.Receiver.Name,
                        Surname = m.Receiver.Surname
                    }
                })
                .FirstOrDefaultAsync(ct);

            if (message is null)
                throw new WebShopNotFoundException("Message with this ID doesn't exist");

            // only the sender, the receiver or an admin can see the message
            if (!currentUser.IsAdmin &&
                currentUser.UserId != message.Sender.Id &&
                currentUser.UserId != message.Receiver.Id)
                throw new WebShopBusinessRuleException("403", "You cannot view this message");

            return message;
        }
    }
}
