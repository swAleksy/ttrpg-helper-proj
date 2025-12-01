using System.Security.Cryptography;
using System.Text;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.Seed;

public static class AuthSeeder
{
    public static async Task SeedAuth(ApplicationDbContext db)
    {
        if (db.Users.Any())
            return;

        var roles = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        };
        db.Roles.AddRange(roles);
        
        var users = new List<User>
        {
            CreateUser("admin1", "admin1@test.pl", "haslo12345"),
            CreateUser("admin2", "admin2@test.pl", "haslo54321"),

            CreateUser("user1", "user1@test.pl", "12345haslo"),
            CreateUser("user2", "user2@test.pl", "54321haslo"),
            CreateUser("user3", "user3@test.pl", "123haslo321")
        };

        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        var userRoles = new List<UserRole>
        {
            new() { UserId = users[0].Id, RoleId = 1 },
            new() { UserId = users[1].Id, RoleId = 1 },
            new() { UserId = users[2].Id, RoleId = 2 },
            new() { UserId = users[3].Id, RoleId = 2 },
            new() { UserId = users[4].Id, RoleId = 2 },
        };

        db.UserRoles.AddRange(userRoles);

        await db.SaveChangesAsync();
    }
    
    
    private static User CreateUser(string userName, string email, string password)
    {
        CreatePasswordHash(password, out var hash, out var salt);

        return new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt
        };
    }
    
    private static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}