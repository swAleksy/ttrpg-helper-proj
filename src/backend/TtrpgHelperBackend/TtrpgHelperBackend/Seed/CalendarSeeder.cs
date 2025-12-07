using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Models.Calendar;

namespace TtrpgHelperBackend.Seed;

public static class CalendarSeeder
{
    public static async Task SeedCalendarEvents(ApplicationDbContext db)
    {
        if (db.CalendarEvents.Any()) return;

        var users = await db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "User"))
            .Take(3)
            .ToListAsync();
        
        if (!users.Any()) return;
        
        var now = DateTime.Now;

        var events = new List<CalendarEvent>
        {
            new CalendarEvent
            {
                UserId = users[0].Id,
                Title = "Prepare campaign scenario",
                Description = "Write NPC profiles and location details for the upcoming session.",
                EventDate = now.AddDays(3).AddHours(18)
            },
            new CalendarEvent
            {
                UserId = users[0].Id,
                Title = "Collect session feedback",
                Description = "Summarize player feedback after the last session.",
                EventDate = now.AddDays(6).AddHours(20)
            },
            new CalendarEvent
            {
                UserId = users[1].Id,
                Title = "Create a new character",
                Description = "Design a mage from the ancient Nethereese empire.",
                EventDate = now.AddDays(4).AddHours(17)
            },
            new CalendarEvent
            {
                UserId = users[1].Id,
                Title = "Buy dice set",
                Description = "Missing d12 and d100 dice for the next session.",
                EventDate = now.AddDays(5).AddHours(15)
            },
            new CalendarEvent
            {
                UserId = users[2].Id,
                Title = "Read rulebook",
                Description = "Study the combat mechanics chapter.",
                EventDate = now.AddDays(2).AddHours(19)
            }
        };
        
        db.CalendarEvents.AddRange(events);
        await db.SaveChangesAsync();
    }
}
