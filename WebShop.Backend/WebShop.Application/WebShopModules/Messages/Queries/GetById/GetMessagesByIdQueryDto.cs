using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Queries.GetById
{
    public class GetMessageByIdQueryDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime RecivedDate { get; set; }
        public GetMessageByIdUserDto Sender { get; set; }
        public GetMessageByIdUserDto Receiver { get; set; }
    }

    public class GetMessageByIdUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
