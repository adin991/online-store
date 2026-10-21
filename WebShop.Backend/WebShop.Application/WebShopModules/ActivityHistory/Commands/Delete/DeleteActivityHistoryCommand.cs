using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.ActivityHistory.Commands.Delete
{
    public class DeleteActivityHistoryCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
