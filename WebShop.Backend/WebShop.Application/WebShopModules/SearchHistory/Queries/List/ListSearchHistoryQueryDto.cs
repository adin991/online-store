using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Queries.List
{
    public class ListSearchHistoryQueryDto
    {
        public int Id { get; init; }
        public int UserId { get; init; }
        public string Content { get; init; } = null!;
        public DateTime Date { get; init; }
    }
}
