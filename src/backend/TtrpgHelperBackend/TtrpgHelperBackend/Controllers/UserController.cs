using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context ;
    public UserController(IUserService userService, ApplicationDbContext context)
    {
        _userService = userService;
        _context =  context;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
       var user = await _userService.Register(request);
       if (user == null)
       {
           return Conflict(new { error = "Username or email already taken " });
       }
       //var token = _authService.CreateToken(user);

       return CreatedAtAction(nameof(Register), new {
           message = "Registration successful",
           username = user.UserName
       });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
            var token = await _userService.Login(request);

            if (token == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            // Return the (placeholder) token
            return Ok(new { Token = token });
    }

    [Authorize]
    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UserRegisterDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userNameClaim = User.FindFirst(ClaimTypes.Name);

        if (userIdClaim == null || userNameClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("Invalid token claims.");
        }
        
        var user = await _context.Users
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(c => c.Id == userId);
        
        if  (user == null)
            return Unauthorized("User not found.");

        if (user.UserName != request.UserName)
        {
            user.UserName = request.UserName;
        }

        if (user.Email != request.Email)
        {
            user.Email = request.Email;
        }
        await _context.SaveChangesAsync();
        // todo: zmiana hasla, zmiana admin/user, zaimplementowanie metod w serwisie 
        return Ok("User updated successfully.");
    }
}