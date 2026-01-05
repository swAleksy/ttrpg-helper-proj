using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;

namespace TtrpgHelperBackend.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardForUserAsync(int userId);
    }

    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardForUserAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new Exception("User not found.");
            
            var characters = await _context.Characters
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .Select(c => new DashboardCharacterSummaryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    RaceName = c.Race.Name,
                    ClassName = c.Class.Name,
                    Level = c.Level,
                })
                .ToListAsync();
            
            var dashboard = new DashboardDto
            {
                UserName = user.UserName,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Email = user.Email,
                Characters = characters,
            };

            return dashboard;
        }
    }
}
