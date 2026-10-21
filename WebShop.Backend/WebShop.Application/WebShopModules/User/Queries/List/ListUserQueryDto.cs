namespace WebShop.Application.WebShopModules.User.Queries.List
{
    public class ListUserQueryDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required bool IsAdmin { get; set; }
        public required bool IsEnabled { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
    }
}
