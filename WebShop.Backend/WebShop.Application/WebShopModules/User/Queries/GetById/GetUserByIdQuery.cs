namespace WebShop.Application.WebShopModules.User.Queries.GetById
{
    public class GetUserByIdQuery : IRequest<GetUserByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
