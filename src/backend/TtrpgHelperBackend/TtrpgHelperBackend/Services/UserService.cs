using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Models.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace TtrpgHelperBackend.Services;

public interface IUserService
{
    Task<User?> Register(UserRegisterDto request);
    Task<TokenResponseDto?> Login(UserLoginDto request);
    Task<TokenResponseDto?> RefreshTokens(TokenRefreshDto request);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }

    public async Task<User?> Register(DTOs.UserRegisterDto request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserName.ToLower() == request.UserName.ToLower() || u.Email.ToLower() == request.Email.ToLower());

        if (userExists)
            return null;
        
        // zwraca passwordHash / passwordSalt
        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
        
        var newUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        Role? roleToAssign;
        if (request.IsAdminRequest)
        {
            roleToAssign = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        }
        else
        {
            roleToAssign = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        }
        if (roleToAssign == null)
        {
            throw new Exception("Log error: Roles were not seeded properly!");
            return null; 
        }

        _context.UserRoles.Add(new UserRole
        {
            UserId = newUser.Id,
            RoleId = roleToAssign.Id            
        });
        
        await _context.SaveChangesAsync();
        return newUser;
    }

    public async Task<TokenResponseDto?> Login(UserLoginDto request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == request.Username);
        
            if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null; // Invalid credentials
            }
            
            return await CreateTokenResponse(user);
    }

    private async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        var response = new TokenResponseDto
        {
            Token = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
        return response;
    }

    // private async Task<User?> ValidateRefreshToken(Guid userId, string refreshToken)
    // {
    //     var user = await _context.Users.FindAsync(userId);
    //     if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
    //     {
    //         return null;
    //     }
    //     
    //     return user;
    // }

    public async Task<TokenResponseDto?> RefreshTokens(TokenRefreshDto request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return null;

        return await CreateTokenResponse(user);
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        return refreshToken;
    }
    
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
        };
        
        claims.AddRange(user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.Name)));
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDesc = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenDesc);
    }
}