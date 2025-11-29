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
        if (db.Campaigns.Any()) return;

        // ====
        // USER
        CreatePasswordHash("test123", out var hash, out var salt);
        
        var user = new User
        {
            Id = 1,
            UserName = "gm@test.pl",
            Email = "gm@test.pl",
            PasswordHash = hash,
            PasswordSalt = salt
        };

        db.Users.Add(user);

        // =========
        // USER ROLE
        db.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = 1
        });

        // ========
        // CAMPAIGN
        var campaign = new Campaign
        {
            Name = "Kampania Testowa",
            Description = "Testowa kampania startowa",
            GameMasterId = user.Id
        };

        db.Campaigns.Add(campaign);
        await db.SaveChangesAsync();

        // ========
        // SCENARIO
        var scenario = new Scenario
        {
            CampaignId = campaign.Id,
            Name = "Pierwsza Przygoda",
            Description = "Wstępna misja testowa"
        };

        db.Scenarios.Add(scenario);
        await db.SaveChangesAsync();

        // =======
        // CHAPTER
        var chapter = new ScenarioChapter
        {
            ScenarioId = scenario.Id,
            Name = "Prolog",
            ContentMarkdown = "Gracze spotykają się w tawernie."
        };

        db.ScenarioChapters.Add(chapter);
        await db.SaveChangesAsync();

        // ====
        // NOTE
        var note = new Note
        {
            CampaignId = campaign.Id,
            Name = "Sekret barmana",
            ContentMarkdown = "Barman widział podejrzaną postać."
        };

        db.Notes.Add(note);
        await db.SaveChangesAsync();

        db.ScenarioChapterNotes.Add(new ScenarioChapterNote
        {
            ScenarioChapterId = chapter.Id,
            NoteId = note.Id
        });

        // ====
        // ITEM
        var item = new Item
        {
            CampaignId = campaign.Id,
            Name = "Stary miecz",
            Description = "Zardzewiały, ale solidny.",
            Type = "Weapon",
            Value = 10
        };

        db.Items.Add(item);
        await db.SaveChangesAsync();

        db.ScenarioChapterItems.Add(new ScenarioChapterItem
        {
            ScenarioChapterId = chapter.Id,
            ItemId = item.Id
        });

        // ========
        // LOCATION
        var location = new Location
        {
            CampaignId = campaign.Id,
            Name = "Tawerna Pod Smokiem",
            Region = "Stare Miasto",
            Description = "Zadymiona karczma pełna szemranych typów."
        };

        db.Locations.Add(location);
        await db.SaveChangesAsync();

        db.ScenarioChapterLocations.Add(new ScenarioChapterLocation
        {
            ScenarioChapterId = chapter.Id,
            LocationId = location.Id
        });

        // ===
        // NPC
        var npc = new Npc
        {
            CampaignId = campaign.Id,
            Name = "Barman Jorek",
            Description = "Wie za dużo.",
            RaceId = 1,
            ClassId = 9,
            Level = 2,
            Strength = 10,
            Dexterity = 14,
            Constitution = 12,
            Intelligence = 13,
            Wisdom = 11,
            Charisma = 15
        };

        db.Npcs.Add(npc);
        await db.SaveChangesAsync();

        db.ScenarioChapterNpcs.Add(new ScenarioChapterNpc
        {
            ScenarioChapterId = chapter.Id,
            NpcId = npc.Id
        });

        // =========
        // NPC SKILL
        db.NpcSkills.AddRange(
            new NpcSkill
            {
                NpcId = npc.Id,
                Name = "Persuasion",
                Description = "Dobra gadka",
                Value = 3
            },
            new NpcSkill
            {
                NpcId = npc.Id,
                Name = "Insight",
                Description = "Czytanie ludzi",
                Value = 2
            }
        );

        await db.SaveChangesAsync();
    }
    
    static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}
