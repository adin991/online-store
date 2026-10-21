namespace WebShop.Application.WebShopModules.User.Queries.GetById
{
    public class GetUserByIdQueryDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public string? Pfp { get; set; }
        public required Gender Gender { get; set; }
        public required bool IsAdmin { get; set; }
    }
}
