using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegister request)
    {
       var user = await _authService.Register(request);
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
    public async Task<IActionResult> Login(UserLogin request)
    {
            var token = await _authService.Login(request);

            if (token == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            // Return the (placeholder) token
            return Ok(new { Token = token });
    }
    
}