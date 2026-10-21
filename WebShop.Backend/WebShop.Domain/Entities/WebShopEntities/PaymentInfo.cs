using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public enum PaymentType
    {
        Cash,
        Card,
        Check,
        Other,

    }
    public class PaymentInfo : BaseEntity
    {
        public required DateTime DateOfPayment { get; set; }//day of payment
        public required int Amount { get; set; }
        public required PaymentType PaymentType { get; set; }
        public int? CreditCardId { get; set; }
        public CreditCard? CreditCard { get; set; }

    }
}
