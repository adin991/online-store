using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Queries.List
{
    public class ListMessagesQuery : IRequest<PageResult<ListMessagesQueryDto>>
    {
        public string? Search { get; set; }
        public int? ReceiverId { get; set; }
        public int? SenderId { get; set; }
        public bool? IsRead { get; set; }
        public PageRequest Paging { get; set; } = new();
    }
}
