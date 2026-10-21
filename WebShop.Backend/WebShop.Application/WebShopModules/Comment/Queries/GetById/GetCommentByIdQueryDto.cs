using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comment.Queries.GetById
{
    public class GetCommentByIdQueryDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int AdId { get; set; }
        public int UserId { get; set; }
        public GetCommentByIdUserDto User { get; set; }
    }

    public class GetCommentByIdUserDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
