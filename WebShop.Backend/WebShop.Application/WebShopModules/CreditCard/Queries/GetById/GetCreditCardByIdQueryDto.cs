namespace WebShop.Application.WebShopModules.CreditCard.Queries.GetById;

public class GetCreditCardByIdQueryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Last4 { get; set; } = null!;
    public string CardType { get; set; } = null!;
    public int ExpMonth { get; set; }
    public int ExpYear { get; set; }
    public GetCreditCardByIdUserDto User { get; set; } = null!;
}

public class GetCreditCardByIdUserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
}