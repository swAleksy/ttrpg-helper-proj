using System.Security.Cryptography;
using System.Text;
using TtrpgHelperBackend.Models.Authentication;
using TtrpgHelperBackend.Models.Session;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Seed;

public static class DataSeeder
{
    public static async Task Seed(ApplicationDbContext db)
    {
        await AuthSeeder.SeedAuth(db);
        await CharacterSeeder.SeedCharacters(db);
        await SessionSeeder.SeedSessions(db);
        await ResourcesSeeder.SeedResources(db);
    }
}
