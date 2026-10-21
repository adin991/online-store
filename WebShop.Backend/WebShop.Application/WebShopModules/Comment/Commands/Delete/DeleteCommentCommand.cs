using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comments.Commands.Delete
{
    public class DeleteCommentCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
