using WebShop.Application.Modules.Auth.Commands.Login;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebShop.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program>
{
    private static string? _cachedToken;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
    }

    public async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var client = CreateClient();
        if (string.IsNullOrEmpty(_cachedToken))
        {
            // Credentials of the admin user created by DynamicDataSeeder
            // (WebShop.Infrastructure/Database/Seeders/DynamicDataSeeder.cs).
            var loginRequest = new
            {
                Email = "admin@WebShop.local",
                Password = "Admin123!"
            };

            var response = await client.PostAsJsonAsync("api/auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginCommandDto>();
            _cachedToken = loginResponse.AccessToken;
        }
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _cachedToken);
        return client;
    }
}