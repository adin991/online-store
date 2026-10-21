using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comment.Queries.List
{
    public class ListCommentsQueryDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int AdId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public ListCommentsQueryUserDto User { get; set; }
    }

    public class ListCommentsQueryUserDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
