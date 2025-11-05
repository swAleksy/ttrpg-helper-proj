namespace TtrpgHelperBackend.Services;

public interface IUploadService
{
    Task<string> SaveAvatarAsync(string userName, IFormFile file);
}
public class UploadService : IUploadService
{
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _context;

    public UploadService(IWebHostEnvironment env, ApplicationDbContext context)
    {
        _env = env;
        _context = context;
    }
    
    public async Task<string> SaveAvatarAsync(string userName, IFormFile file)
    {
        
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };

        if (!allowedExtensions.Contains(ext))
            throw new ArgumentException("Invalid file type. Only JPG and PNG are allowed.");

        if (file.Length > 2 * 1024 * 1024) // 2 MB limit
            throw new ArgumentException("File too large (max 2MB).");

        // Build file path
        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "avatars");
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        var fileName = $"{userName}{ext}";
        var filePath = Path.Combine(uploadsPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative URL for DB
        return $"/uploads/avatars/{fileName}";
    }
}