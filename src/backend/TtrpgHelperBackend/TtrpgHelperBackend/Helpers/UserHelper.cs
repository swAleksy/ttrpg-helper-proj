using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TtrpgHelperBackend.Helpers;

public class UserHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _db;

    public UserHelper(IHttpContextAccessor httpContextAccessor, ApplicationDbContext db)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _db = db;
    }

    public int? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return null;

        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out int userId)
            ? userId
            : (int?)null;
    }
    
    public string? GetUserName() => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public async Task<bool> IsAdmin()
    {
        var userId = GetUserId();
        if (userId == null) return false;
        
        return await _db.UserRoles.AnyAsync(ur => ur.UserId == userId.Value && ur.RoleId == 1);
    }
}
