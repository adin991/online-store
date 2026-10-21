using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class Role : BaseEntity
    {
        public required string Name { get; set; } = string.Empty;
        public required string Description { get; set; } = string.Empty;
        public List<UserEntity> Users { get; set; } = null!; // one-to-many: a role has many users
    }
}
