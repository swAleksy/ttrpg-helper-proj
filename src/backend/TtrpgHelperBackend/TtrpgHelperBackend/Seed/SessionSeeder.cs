using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Models.Authentication;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Seed;

public class SessionSeeder
{
    public static async Task SeedSessions(ApplicationDbContext db)
    {
        if (db.Sessions.Any()) return;
        
        var characters = db.Characters
            .Include(c => c.User)
            .Take(3)
            .ToList();

        if (!characters.Any()) return;

        
        // ===============
        // -- CAMPAIGNS --
        var campaign1 = new Campaign
        {
            Name = "Lost Ruins of Shadowfall",
            Description = "A mysterious ancient city revealed after a planar anomaly.",
            GameMasterId = 3
        };
        var campaign3 = new Campaign
        {
            Name = "Lost Ruins of 2",
            Description = "A mysterious ancient city revealed after a planar anomaly.",
            GameMasterId = 3
        };
        var campaign4 = new Campaign
        {
            Name = "Lost Ruins of 3",
            Description = "A mysterious ancient city revealed after a planar anomaly.",
            GameMasterId = 3
        };
        var campaign5 = new Campaign
        {
            Name = "Lost Ruins of 4",
            Description = "A mysterious ancient city revealed after a planar anomaly.",
            GameMasterId = 3
        };
        var campaign6 = new Campaign
        {
            Name = "Lost Ruins of 5",
            Description = "A mysterious ancient city revealed after a planar anomaly.",
            GameMasterId = 3
        };
        var campaign7 = new Campaign
        {
            Name = "Lost Ruins of 6",
            Description = "A mysterious ancient city revealed after a planar anomaly.",
            GameMasterId = 3
        };
        var campaign2 = new Campaign
        {
            Name = "Floating Isles of Nethereese",
            Description = "Long lost shard of an empire fallen a long time ago.",
            GameMasterId = 4
        };
        db.Campaigns.AddRange(campaign1, campaign2,campaign3,campaign4,campaign5,campaign6);
        await db.SaveChangesAsync();
        
        // ==============
        // -- SESSIONS --
        var session1 = new Session
        {
            CampaignId = campaign1.Id,
            Name = "Arrival at Shadowfall",
            Description = "The party enters the ruins and meets the first dangers.",
            ScheduledDate = DateTime.Now.AddDays(31),
            Status = "Active"
        };
        var session2 = new Session
        {
            CampaignId = campaign1.Id,
            Name = "Temple of Ash",
            Description = "The party stumbles upon well preserved temple.",
            ScheduledDate = DateTime.Now.AddDays(32),
            Status = "Active"
        };
        var session3 = new Session
        {
            CampaignId = campaign2.Id,
            Name = "Boarding the airship",
            Description = "The party boards the new airship at city's airdeck.",
            ScheduledDate = DateTime.Now.AddDays(33),
            Status = "Active"
        };
        db.Sessions.AddRange(session1, session2, session3);
        await db.SaveChangesAsync();
        
        // =====================
        // -- SESSION PLAYERS --
        var sessions = db.Sessions
            .Include(s => s.Campaign)
            .ToList();

        var uniquePlayers = characters
            .Select(c => c.UserId)
            .Distinct()
            .ToList();
        
        foreach (var session in sessions)
        {
            foreach (var userId in uniquePlayers)
            {
                // if (userId == session.Campaign.GameMasterId) 
                //     continue;

                db.SessionPlayers.Add(new SessionPlayer
                {
                    PlayerId = userId,
                    SessionId = session.Id
                });
            }
        }
        await db.SaveChangesAsync();
        
        // ====================
        // -- SESSION EVENTS --
        var events = new[]
        {
            new SessionEvent
            {
                SessionId = session1.Id,
                UserId = 3,
                Type = SessionEventType.DiceRoll,
                DataJson = "{ \"dice\": \"d20\", \"result\": 14 }",
                Timestamp = DateTime.Now.AddMinutes(10),
            },

            new SessionEvent
            {
                SessionId = session1.Id,
                UserId = 5,
                Type = SessionEventType.DiceRoll,
                DataJson = "{ \"dice\": \"d100\", \"result\": 55 }",
                Timestamp = DateTime.Now.AddMinutes(12),
            },

            new SessionEvent
            {
                SessionId = session2.Id,
                UserId = 4,
                Type = SessionEventType.DiceRoll,
                DataJson = "{ \"dice\": \"d10\", \"result\": 6 }",
                Timestamp = DateTime.Now.AddMinutes(14),
            },

            new SessionEvent
            {
                SessionId = session3.Id,
                UserId = 4,
                Type = SessionEventType.DiceRoll,
                DataJson = "{ \"dice\": \"d6\", \"result\": 3 }",
                Timestamp = DateTime.Now.AddMinutes(20),
            }
        };
        db.SessionEvents.AddRange(events);
        await db.SaveChangesAsync();
    }
}
