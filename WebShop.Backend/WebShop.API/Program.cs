using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;
using WebShop.API;
using WebShop.API.Middleware;
using WebShop.Application;
using WebShop.Domain.Entities.WebShopEntities;
using WebShop.Infrastructure;

public partial class Program
{
    private static async Task Main(string[] args)
    {

        //
        // 0) Bootstrap logger (very early, no full config yet)
        //
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console() // minimal sink so we see startup errors
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting WebShop API...");

            //
            // 1) Standard builder (includes appsettings.json, appsettings.{ENV}.json,
            //    environment variables, user-secrets (Dev), and command-line args)
            //
            var builder = WebApplication.CreateBuilder(args);

            // 2) Promote Serilog to full configuration from builder.Configuration
            //    (reads "Serilog" section from appsettings + ENV overrides)
            //
            builder.Host.UseSerilog((ctx, services, cfg) =>
            {
                cfg.ReadFrom.Configuration(ctx.Configuration)   // Serilog section in appsettings
                   .ReadFrom.Services(services)                 // DI enrichers if any
                   .Enrich.FromLogContext()
                   .Enrich.WithThreadId()
                   .Enrich.WithProcessId()
                   .Enrich.WithMachineName();
            });

            // Optional: remove default providers to have only Serilog
            builder.Logging.ClearProviders();

            // ---------------------------------------------------------
            // 3. Layer registrations
            // ---------------------------------------------------------
            builder.Services
                .AddAPI(builder.Configuration, builder.Environment)
                .AddInfrastructure(builder.Configuration, builder.Environment)
                .AddApplication();
            // Enums as strings in JSON + Swagger
            builder.Services
                .AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddSwaggerGen(c =>
            {
                c.MapType<AdStatusType>(() => new OpenApiSchema
                {
                    Type = "string",
                    Enum = Enum.GetNames(typeof(AdStatusType))
                        .Select(v => new OpenApiString(v))
                        .Cast<IOpenApiAny>()
                        .ToList()
                });
            });

            // CORS policy to allow Angular dev server access
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev",
                    policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:4200") // more origins can be passed as a string array
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            // wwwroot must exist BEFORE builder.Build() - otherwise ASP.NET Core sets
            // WebRootFileProvider to NullFileProvider and UseStaticFiles would never
            // find files saved to disk later.
            var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads", "ads");
            Directory.CreateDirectory(uploadsPath);

            var app = builder.Build();

            // ---------------------------------------------------------
            // 4. Middleware pipeline
            // ---------------------------------------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            var isDesignTime = Environment.GetCommandLineArgs().Any(arg => arg.Contains("ef"));
            if (!isDesignTime)
            {
                await app.Services.InitializeDatabaseAsync(app.Environment);
            }
            // Global exception handler (IExceptionHandler)
            app.UseExceptionHandler();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            // UseCors must run before HttpsRedirection and Authentication
            app.UseCors("AllowAngularDev");

            // Sets CultureInfo.CurrentCulture per request from the Accept-Language header
            app.UseRequestLocalization();

            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            Log.Information("WebShop API started successfully.");
            app.Run();

            

            

        }
        catch (HostAbortedException)
        {
            // EF Core tools abort the host after they obtain the DbContext.
            // This is not a runtime error - just exit quietly.
            Log.Information("Host aborted by EF Core tooling (design-time) - its ok.");
        }
        catch (Exception ex)
        {
            // Any startup failure will be logged here
            Log.Fatal(ex, "WebShop API terminated unexpectedly.");
        }
        finally
        {
            // Ensure all logs are flushed before the app exits
            Log.CloseAndFlush();
        }
    }
}
