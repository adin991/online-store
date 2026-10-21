using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comment.Queries.List
{
    public class ListCommentsQuery : IRequest<PageResult<ListCommentsQueryDto>>
    {
        public string? Search { get; set; }
        public int? AdId { get; set; }
        public PageRequest Paging { get; set; } = new();
    }
}
