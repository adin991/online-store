namespace WebShop.Application.Common.Exceptions;

public sealed class WebShopConflictException : Exception
{
    public WebShopConflictException(string message) : base(message) { }
}
