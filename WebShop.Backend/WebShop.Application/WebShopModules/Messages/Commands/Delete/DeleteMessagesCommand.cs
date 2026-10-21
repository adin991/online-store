using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Commands.Delete
{
    public class DeleteMessageCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
