using TtrpgHelperBackend.Helpers;

namespace TtrpgHelperBackend.Services;

public interface IUploadService
{
    Task<ServiceResponseH<bool>> UpdateUserAvatarAsync(int userId, string avatarUrl);
}
public class UploadService : IUploadService
{
    private readonly ApplicationDbContext _context;

    public UploadService( ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ServiceResponseH<bool>> UpdateUserAvatarAsync(int userId, string avatarUrl)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return new ServiceResponseH<bool> { Success = false, Message = "User not found." };
        }
        string? oldAvatarUrl = user.AvatarUrl;

        user.AvatarUrl = avatarUrl;
        await _context.SaveChangesAsync();
        
        if (!string.IsNullOrEmpty(oldAvatarUrl))
        {
            var oldFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                oldAvatarUrl.TrimStart('/')
            );

            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }
        }
        return new ServiceResponseH<bool> { Success = true };
    }
}