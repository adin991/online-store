using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Queries.GetById
{
    public class GetSearchHistoryByIdQueryHandler(IAppDbContext context)
        : IRequestHandler<GetSearchHistoryByIdQuery, GetSearchHistoryByIdQueryDto>
    {
        public async Task<GetSearchHistoryByIdQueryDto> Handle(GetSearchHistoryByIdQuery request, CancellationToken ct)
        {
            var searchHistory = await context.SearchHistorys
                .AsNoTracking()
                .Where(s => s.Id == request.Id)
                .Select(s => new GetSearchHistoryByIdQueryDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Content = s.Content,
                    Date = s.Date,
                    User = new GetSearchHistoryByIdUserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Surname = s.User.Surname
                    }
                })
                .FirstOrDefaultAsync(ct);

            if (searchHistory is null)
                throw new WebShopNotFoundException("Search history with this ID doesn't exist");

            return searchHistory;
        }
    }
}
