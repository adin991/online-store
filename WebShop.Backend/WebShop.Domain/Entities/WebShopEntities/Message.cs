using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class Message : BaseEntity
    {
        public required int SenderId { get; set; } // user
        public required UserEntity Sender { get; set; } = null!;
        public required int ReceiverId { get; set; } // drugi user
        public required UserEntity Receiver { get; set; } = null!;
        public required DateTime RecivedDate { get; set; }
        public required string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;

    }
}
