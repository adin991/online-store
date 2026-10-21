using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comments.Commands.Create
{
    public class CreateCommentCommand : IRequest<int>
    {
        public required string Content { get; set; }
        public required int AdId { get; set; }
    }
}
