using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Queries.List
{
    public class ListMessagesQueryDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime RecivedDate { get; set; }
        public ListMessagesQueryUserDto Sender { get; set; }
        public ListMessagesQueryUserDto Receiver { get; set; }
    }

    public class ListMessagesQueryUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
