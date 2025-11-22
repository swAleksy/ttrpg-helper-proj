using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController: ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ApplicationDbContext _context;

    public DashboardController(IDashboardService dashboardService, ApplicationDbContext context)
    {
        _dashboardService = dashboardService;
        _context = context;
    }
    
    [Authorize]
    [HttpGet("Dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid token claims.");
        }
        
        try
        {
            var dashboard = await _dashboardService.GetDashboardForUserAsync(userId);
            
            return Ok(dashboard);
        }
        catch(Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}