using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;
    private readonly ApplicationDbContext _context;
    public UploadController(IUploadService uploadService, ApplicationDbContext context)
    {
        _uploadService = uploadService;
        _context = context;
    }
    
    [Authorize]
    [HttpPost("uploadavatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // 1. Validate File Type (Security)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type.");

        if (file.Length > 2 * 1024 * 1024) // 2 MB limit
            return BadRequest("File too large (max 2MB).");
        
        // 2. Generate Unique Filename to prevent collisions
        var fileName = $"{Guid.NewGuid()}{extension}";
    
        // 3. Define Path (e.g., wwwroot/uploads/avatars)
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, fileName);

        // 4. Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 5. Update User in DB (Assuming you have a service method for this)
        // The URL should be relative, e.g., "/uploads/avatars/guid.jpg"
        var avatarUrl = $"/uploads/avatars/{fileName}";
    
        var result = await _uploadService.UpdateUserAvatarAsync(userId.Value, avatarUrl);
    
        if (!result.Success) return BadRequest("Could not save to database");

        return Ok(new { avatarUrl });
    }
    
    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        int? userIdResult = null; 

        if (claim == null) return userIdResult;
        if (int.TryParse(claim.Value, out int userId))
            userIdResult = userId;
        return userIdResult;
    }
}