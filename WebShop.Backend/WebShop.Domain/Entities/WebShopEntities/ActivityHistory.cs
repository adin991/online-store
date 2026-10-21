using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public enum ActivityType
    {
        Login,
        Logout,
        ViewedProduct,
        AddedToCart,
        Purchased,
        Refunded,
        SentMessage,
    }
    public class ActivityHistory : BaseEntity
    {
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public required ActivityType Activity { get; set; }
    }
}
