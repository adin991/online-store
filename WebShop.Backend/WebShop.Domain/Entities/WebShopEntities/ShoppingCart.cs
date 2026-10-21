using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class ShoppingCart : BaseEntity
    {
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public required List<SCItem> Items { get; set; } = new();
        public required DateTime Date { get; set; }
    }
}
