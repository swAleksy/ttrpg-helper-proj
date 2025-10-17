using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace TtrpgHelperBackend.Services;

public interface IAuthService
{
    Task<User?> Register(UserRegister request);
    Task<string?> Login(UserLogin request);
    
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration, ApplicationDbContext context)
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

    public async Task<User?> Register(DTOs.UserRegister request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserName.ToLower() == request.UserName.ToLower() || u.Email.ToLower() == request.Email.ToLower());

        if (userExists)
            return null;
        
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

    public async Task<string?> Login(UserLogin request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == request.Username);
        
            if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null; // Invalid credentials
            }

            return CreateToken(user);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
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