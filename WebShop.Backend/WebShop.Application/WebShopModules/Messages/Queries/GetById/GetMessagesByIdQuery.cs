using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Queries.GetById
{
    public class GetMessageByIdQuery : IRequest<GetMessageByIdQueryDto>
    {
        public required int Id { get; set; }
    }
}
