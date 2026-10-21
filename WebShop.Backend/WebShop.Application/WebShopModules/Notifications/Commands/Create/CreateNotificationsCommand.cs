using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Notifications.Commands.Create
{
    public class CreateNotificationsCommand : IRequest<int>
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
}
