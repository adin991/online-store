namespace WebShop.Tests.TestSupport;

/// <summary>
/// Test double for IAppCurrentUser - lets a test set who the "currently logged-in"
/// user is (Id, Admin flag, authentication).
/// </summary>
internal sealed class FakeCurrentUser : IAppCurrentUser
{
    public int? UserId { get; set; }
    public string? Email { get; set; }
    public bool IsAuthenticated { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsManager { get; set; }
    public bool IsEmployee { get; set; }
}
