using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class Notification : BaseEntity
    {
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required bool IsRead { get; set; } = false;
        public required DateTime DateOfRead { get; set; }
    }
}
