using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class SearchHistory : BaseEntity
    {
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!; 
        public required string Content { get; set; } = string.Empty;
        public required DateTime Date { get; set; }
    }
}
