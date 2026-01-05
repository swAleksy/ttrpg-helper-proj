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
    public async Task<ActionResult<TokenResponseDto>> Register(UserRegisterDto request)
    {
       var user = await _userService.Register(request);
       if (user == null)
       {
           return Conflict(new { error = "Username or email already taken " });
       }
       
       var result = await _userService.Login(new UserLoginDto
       {
           Password = request.Password,
           Username = request.UserName
       });

       if (result == null)
       {
           return Unauthorized(new { Message = "Login failed after registration" });
       }
       
       // return CreatedAtAction(nameof(Register), new {
       //     message = "Registration successful",
       //     username = user.UserName
       // });
       return Ok(result); 
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserLoginDto request)
    {
            var result = await _userService.Login(request);

            if (result == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." }); 
            }

            // Return the (placeholder) token
            return Ok(result);
    }


    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(TokenRefreshDto request)
    {
        var result = await _userService.RefreshTokens(request);
        if (result == null || result.Token == null || result.RefreshToken == null)
        {
            return Unauthorized(new { error = "Invalid token or password." });
        }
        return Ok(result);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> ChangeProfile([FromBody] UpdateUserProfileDto request)
    {
        var userId = GetUserId(); 
    
        if (userId == null)
            return Unauthorized();
        
        var result = await _userService.UpdateUserProfileAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest("BE" + result.Message );
        }
        
        return Ok( "Password changed successfully."); 
    }
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userId = GetUserId(); 
    
        if (userId == null)
            return Unauthorized("BE user not found");
        
        var result = await _userService.ChangePasswordAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest(result.Message );
        }
        
        return Ok( "Password changed successfully.");
    }
    
    [Authorize]
    [HttpGet("Me")]
    public async Task<ActionResult<UserInfoDto>> GetMe() 
    {
        var userId = GetUserId();
    
        if (userId == null)
        {
            return BadRequest("Token issue: Claim not found"); 
        }

        var user = await _userService.GetUserById(userId.Value);

        if (user == null)
        {
            return NotFound("User ID from token not found in DB.");
        }

        var userData = new UserInfoDto
        {
            Id  = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
        };
        return Ok(userData);
    }

    [HttpGet("info/{id}")]
    public async Task<ActionResult<UserInfoDto>> GetInfo(int id)
    {
        var user = await _userService.GetUserById(id);

        if (user == null)
        {
            return NotFound("User ID from token not found in DB.");
        }

        var userData = new UserInfoDto
        {
            Id  = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
        };
        
        return userData;
    }
    
    [Authorize]
    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser()
    {
        var userId = GetUserId(); 

        if (userId == null)
        {
            return BadRequest("Token issue: Claim not found"); 
        }

        var result = await _userService.DeleteUserAsync(userId.Value);

        if (!result)
        {
            return NotFound("User ID from token not found in DB.");
        }

        return Ok("User and all related data (friends, messages) deleted successfully.");
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
    
    private string GetUserName()
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        
        if (string.IsNullOrEmpty(userName))
            throw new Exception("User ID claim (NameIdentifier) is missing from the principal.");
        return userName;
    }
}