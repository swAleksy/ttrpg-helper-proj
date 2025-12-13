namespace TtrpgHelperBackend.Seed;

public static class DataSeeder
{
    public static async Task Seed(ApplicationDbContext db)
    {
        await AuthSeeder.SeedAuth(db);
        await CharacterSeeder.SeedCharacters(db);
        await SessionSeeder.SeedSessions(db);
        await ResourcesSeeder.SeedResources(db);
        await CalendarSeeder.SeedCalendarEvents(db);
    }
}
