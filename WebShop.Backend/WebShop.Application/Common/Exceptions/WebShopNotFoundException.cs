namespace WebShop.Application.Common.Exceptions;

public sealed class WebShopNotFoundException : Exception
{
    public WebShopNotFoundException(string message) : base(message) { }
}
