using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class DeliveryCompany : BaseEntity
    {

        public string Name { get; set; }
        //more data to be added
        public List<Delivery> Deliveries { get; set; } = new();

    }
}
