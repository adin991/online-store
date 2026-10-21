namespace WebShop.Application.WebShopModules.CreditCard.Queries.List;

public class ListCreditCardQueryDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Last4 { get; init; } = null!;
    public string CardType { get; init; } = null!;
    public int ExpMonth { get; init; }
    public int ExpYear { get; init; }
    public ListCreditCardQueryUserDto User { get; init; } = null!;
}

public class ListCreditCardQueryUserDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Surname { get; init; } = null!;
}