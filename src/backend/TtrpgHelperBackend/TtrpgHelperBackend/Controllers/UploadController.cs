using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController: ControllerBase
{
    private readonly UploadService _uploadService;
    private readonly ApplicationDbContext _context;
    
    public UploadController(UploadService uploadService, ApplicationDbContext context)
    {
        _uploadService = uploadService;
        _context = context;
    }
    
    [HttpPost("uploadAvatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userNameClaim = User.FindFirst(ClaimTypes.Name);
        
        if (userNameClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User name claim not found in token.");
        }

        var userName = userNameClaim.Value;
        try
        {
            var avatarUrl = await _uploadService.SaveAvatarAsync(userName, file);
            var user = await _context.Users
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(c => c.Id == userId && c.UserName == userName);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            
            user.AvatarUrl = avatarUrl;
            await _context.SaveChangesAsync();
            return Ok(new { AvatarUrl = avatarUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}