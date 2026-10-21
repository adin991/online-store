using WebShop.Application.Common.Exceptions;

namespace WebShop.API.Controllers
{
    public class UploadFileResultDto
    {
        public required string Url { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IWebHostEnvironment env) : ControllerBase
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif"
        };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        [HttpPost("upload")]
        public async Task<ActionResult<UploadFileResultDto>> Upload(IFormFile file, CancellationToken ct)
        {
            if (file is null || file.Length == 0)
                throw new WebShopBusinessRuleException("400", "No file provided");

            if (file.Length > MaxFileSizeBytes)
                throw new WebShopBusinessRuleException("400", "File too large (max 5 MB)");

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                throw new WebShopBusinessRuleException("400", "Only image files are allowed (jpg, png, webp, gif)");

            var webRoot = string.IsNullOrEmpty(env.WebRootPath)
                ? Path.Combine(env.ContentRootPath, "wwwroot")
                : env.WebRootPath;
            var uploadsRoot = Path.Combine(webRoot, "uploads", "ads");
            Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            return Ok(new UploadFileResultDto { Url = $"/uploads/ads/{fileName}" });
        }
    }
}
