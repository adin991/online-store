using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Notifications.Queries.List
{
    public class ListNotificationsQuery : BasePagedQuery<ListNotificationsQueryDto>
    {
        public string? SearchTtile { get; init; }
    }
}
