using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Models.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TtrpgHelperBackend.Helpers;
namespace TtrpgHelperBackend.Services;

public interface IUserService
{
    Task<User?> Register(UserRegisterDto request);
    Task<TokenResponseDto?> Login(UserLoginDto request);
    Task<TokenResponseDto?> RefreshTokens(TokenRefreshDto request);
    Task<User?> GetUserById(int id);
    
    Task<ServiceResponseH<bool>> ChangePasswordAsync(int userId, ChangePasswordDto request);
    Task<ServiceResponseH<User>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto request);
    Task<bool> DeleteUserAsync(int userId);
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

    public async Task<User?> Register(UserRegisterDto request)
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
            PasswordSalt = passwordSalt,
            AvatarUrl = request.AvatarUrl,
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
                return null; 
            }
            
            return await CreateTokenResponse(user);
    }
    
    public async Task<User?> GetUserById(int id)
    {
        var user = await _context.Users
            .Include(r => r.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        return user;
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
            .ThenInclude(ur => ur.Role)
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
    
    public async Task<ServiceResponseH<bool>> ChangePasswordAsync(int userId, ChangePasswordDto request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return new ServiceResponseH<bool> { Success = false, Message = "User not found." };
        }

        // 1. Verify the OLD password using your existing private method
        if (!VerifyPasswordHash(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
        {
            return new ServiceResponseH<bool> { Success = false, Message = "Incorrect current password." };
        }

        // 2. Hash the NEW password using your existing private method
        CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

        // 3. Update the user entity
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        await _context.SaveChangesAsync();

        return new ServiceResponseH<bool> { Data = true, Message = "Password changed successfully." };
    }
    public async Task<ServiceResponseH<User>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new ServiceResponseH<User> { Success = false, Message = "User not found." };
        }

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            if (user.UserName != request.UserName)
            {
                var exists = await _context.Users.AnyAsync(u => u.UserName == request.UserName && u.Id != userId);
                if (exists)
                    return new ServiceResponseH<User> { Success = false, Message = "Username already taken." };

                user.UserName = request.UserName;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            user.Email = request.Email;
        }
    
        await _context.SaveChangesAsync();

        return new ServiceResponseH<User> { Data = user, Message = "Profile updated successfully." };
    }
    
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var userToDelete = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (userToDelete == null)
        {
            return false;
        }
        
        var campaigns = await _context.Campaigns
            .Where(c => c.GameMasterId == userId)
            .ToListAsync();

        if (campaigns.Any())
        {
            _context.Campaigns.RemoveRange(campaigns);
        }
        
        var characters = await _context.Characters
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (characters.Any())
        {
            _context.Characters.RemoveRange(characters);
        }
        
        var relatedFriendships = await _context.Friendships
            .Where(f => f.SourceUserId == userId || f.TargetUserId == userId)
            .ToListAsync();

        if (relatedFriendships.Any())
        {
            _context.Friendships.RemoveRange(relatedFriendships);
        }

        var relatedMessages = await _context.ChatMessages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .ToListAsync();

        if (relatedMessages.Any())
        {
            _context.ChatMessages.RemoveRange(relatedMessages);
        }

        // 4. (Opcjonalnie) USUWANIE POSTACI (Characters)
        // Jeśli masz tabelę Characters i tam też jest Restrict/Cascade, warto to obsłużyć:
        // var characters = await _context.Characters.Where(c => c.UserId == userId).ToListAsync();
        // _context.Characters.RemoveRange(characters);

        // 5. Usuwanie ról użytkownika
        if (userToDelete.UserRoles != null && userToDelete.UserRoles.Any())
        {
            _context.UserRoles.RemoveRange(userToDelete.UserRoles);
        }

        // 6. Finalne usunięcie użytkownika
        _context.Users.Remove(userToDelete);

        // Zapisanie wszystkich zmian w jednej transakcji
        await _context.SaveChangesAsync();
    
        return true;
    }
}