using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Notifications.Queries.GetById
{
    public class GetNotificationsByIdQuery : IRequest<GetNotificationsByIdQueryDto>
    {
        public required int Id { get; set; }
    }
}
