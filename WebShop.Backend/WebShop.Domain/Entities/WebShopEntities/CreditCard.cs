using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class CreditCard : BaseEntity
    {
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public required string Last4 { get; set; } = null!;
        public required string CardType { get; set; } = null!;
        public required int ExpMonth { get; set; }
        public required int ExpYear { get; set; }
    }
}
