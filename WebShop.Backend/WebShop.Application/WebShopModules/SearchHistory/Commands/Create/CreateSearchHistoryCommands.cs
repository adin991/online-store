using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Commands.Create
{
    public class CreateSearchHistoryCommand : IRequest<int>
    {
        public required string Content { get; set; }
    }
}
