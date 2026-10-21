using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Queries.GetById
{
    public class GetSearchHistoryByIdQuery : IRequest<GetSearchHistoryByIdQueryDto>
    {
        public required int Id { get; set; }
    }
}
