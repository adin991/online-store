using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Commands.Create
{
    public class CreateMessageCommand : IRequest<int>
    {
        // sender id is not required because it will be taken from the token
        public required int ReceiverId { get; set; }
        public required string Content { get; set; }
    }
}
