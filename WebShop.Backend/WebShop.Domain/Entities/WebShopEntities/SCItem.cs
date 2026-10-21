using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class SCItem : BaseEntity
    {
        public required int AdId { get; set; }
        public Ad Ad { get; set; } = null!;
        public required int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; } = null!;
        public required int Amount { get; set; } = 1;

    }
}
