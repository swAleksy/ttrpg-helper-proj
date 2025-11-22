using System.Security.Claims;

namespace TtrpgHelperBackend.Helpers;

public class UserHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
}