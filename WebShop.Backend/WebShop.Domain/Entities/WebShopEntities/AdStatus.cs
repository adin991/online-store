using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public enum AdStatusType
    {
        Live,
        Closed,
        AwaitingReview,
        Removed,
        ShadowBanned,
        Boosted,
    }
    public class AdStatus : BaseEntity
    {
        public int AdId { get; set; }
        public Ad Ad { get; set; } = null!;
        public required AdStatusType Status { get; set; }
    }
}
