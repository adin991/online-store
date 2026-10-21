using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Commands.Update
{
    public class UpdateSearchHistoryCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public required string Content { get; set; }
    }
}
