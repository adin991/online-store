using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using WebShop.API.Controllers;

namespace WebShop.Tests.FilesTests.UnitTests;

public class FilesUnitTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly FakeWebHostEnvironment _env;

    public FilesUnitTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "WebShopTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempRoot);
        _env = new FakeWebHostEnvironment { ContentRootPath = _tempRoot, WebRootPath = "" };
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
            Directory.Delete(_tempRoot, recursive: true);
    }

    private static FormFile CreateFormFile(string fileName, int sizeInBytes, string contentType = "image/jpeg")
    {
        var content = new byte[sizeInBytes];
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, sizeInBytes, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    [Fact]
    public async Task Upload_ShouldSaveFileAndReturnUrl_WhenImageIsValid()
    {
        var controller = new FilesController(_env);
        var file = CreateFormFile("image.jpg", 1024);

        var result = await controller.Upload(file, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<UploadFileResultDto>(ok.Value);
        Assert.StartsWith("/uploads/ads/", dto.Url);
        Assert.EndsWith(".jpg", dto.Url);

        var savedPath = Path.Combine(_tempRoot, "wwwroot", "uploads", "ads", Path.GetFileName(dto.Url));
        Assert.True(File.Exists(savedPath));
    }

    [Fact]
    public async Task Upload_ShouldThrowBusinessRule_WhenFileIsEmpty()
    {
        var controller = new FilesController(_env);
        var file = CreateFormFile("empty.jpg", 0);

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => controller.Upload(file, CancellationToken.None));
    }

    [Fact]
    public async Task Upload_ShouldThrowBusinessRule_WhenFileExceedsMaxSize()
    {
        var controller = new FilesController(_env);
        const int sixMegabytes = 6 * 1024 * 1024;
        var file = CreateFormFile("veliko.jpg", sixMegabytes);

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => controller.Upload(file, CancellationToken.None));
    }

    [Fact]
    public async Task Upload_ShouldThrowBusinessRule_WhenExtensionNotAllowed()
    {
        var controller = new FilesController(_env);
        var file = CreateFormFile("dokument.exe", 1024, contentType: "application/octet-stream");

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => controller.Upload(file, CancellationToken.None));
    }

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ApplicationName { get; set; } = "WebShop.Tests";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "";
        public string EnvironmentName { get; set; } = "IntegrationTests";
    }
}
