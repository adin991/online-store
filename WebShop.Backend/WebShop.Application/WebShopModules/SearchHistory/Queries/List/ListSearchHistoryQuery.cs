using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Queries.List
{
    public class ListSearchHistoryQuery : BasePagedQuery<ListSearchHistoryQueryDto>
    {
        public string? SearchContent { get; set; }
    }
}
