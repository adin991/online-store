namespace WebShop.Application.WebShopModules.User.Queries.List
{
    public class ListUserQuery : BasePagedQuery<ListUserQueryDto>
    {
        /// <summary>Matches name, surname or email.</summary>
        public string? Search { get; set; }

        /// <summary>true = only admins, false = only regular users, null = all.</summary>
        public bool? IsAdmin { get; set; }

        /// <summary>"name" (default), "email" or "newest".</summary>
        public string? SortBy { get; set; }
    }
}
