using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comment.Queries.GetById
{
    public class GetCommentByIdQuery : IRequest<GetCommentByIdQueryDto>
    {
        public required int Id { get; set; }
    }
}
