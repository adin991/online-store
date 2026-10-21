using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Commands.Delete
{
    public class DeleteSearchHistoryCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
