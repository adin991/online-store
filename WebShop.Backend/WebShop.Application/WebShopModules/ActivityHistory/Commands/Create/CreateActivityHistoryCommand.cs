using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.ActivityHistory.Commands.Create
{
    public class CreateActivityHistoryCommand : IRequest<int>
    {
        public required ActivityType Activity { get; set; }
    }
}
