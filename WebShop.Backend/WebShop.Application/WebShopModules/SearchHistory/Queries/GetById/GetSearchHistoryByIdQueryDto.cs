using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Queries.GetById
{
    public class GetSearchHistoryByIdQueryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Date { get; set; }
        public GetSearchHistoryByIdUserDto User { get; set; } = null!;
    }

    public class GetSearchHistoryByIdUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
    }
}
