using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Models.Resource;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Seed;

public static class ResourcesSeeder
{
    public static async Task SeedResources(ApplicationDbContext db)
    {
        if (db.Scenarios.Any()) return;

        var campaigns = await db.Campaigns.ToListAsync();
        if (!campaigns.Any()) return;

        foreach (var campaign in campaigns)
        {
            // =========
            // SCENARIOS
            var scenario1 = new Scenario
            {
                CampaignId = campaign.Id,
                Name = "The Forgotten Crypt",
                Description = "An old burial site recently uncovered by an earthquake."
            };
            var scenario2 = new Scenario
            {
                CampaignId = campaign.Id,
                Name = "Bandits of Black Hollow",
                Description = "Raids on nearby villages lead to a dark forest hideout."
            };
            db.Scenarios.AddRange(scenario1, scenario2);
            await db.SaveChangesAsync();
            
            // ========
            // CHAPTERS
            var chapter1 = new ScenarioChapter
            {
                ScenarioId = scenario1.Id,
                Name = "Arrival at the Crypt",
                ContentMarkdown = "The party discovers the collapsed entrance."
            };
            var chapter2 = new ScenarioChapter
            {
                ScenarioId = scenario1.Id,
                Name = "Hall of Bones",
                ContentMarkdown = "Ancient skeletons lie shattered across the floor."
            };
            var chapter3 = new ScenarioChapter
            {
                ScenarioId = scenario2.Id,
                Name = "Forest Ambush",
                ContentMarkdown = "Bandits strike from the trees along the road."
            };
            db.ScenarioChapters.AddRange(chapter1, chapter2, chapter3);
            await db.SaveChangesAsync();
            
            // =====
            // NOTES
            var note1 = new Note
            {
                CampaignId = campaign.Id,
                Name = "Sarcophagus Inscription",
                ContentMarkdown = "Here rests Aramil the Unbroken, defender of the realm."
            };
            var note2 = new Note
            {
                CampaignId = campaign.Id,
                Name = "Bandit Map",
                ContentMarkdown = "A rough map marking forest trails and hidden camps."
            };
            db.Notes.AddRange(note1, note2);
            await db.SaveChangesAsync();
            
            // =====
            // ITEMS
            var item1 = new Item
            {
                CampaignId = campaign.Id,
                Name = "Ancient Blade",
                Description = "Rust-covered sword of forgotten make.",
                Type = "Weapon",
                Value = 75
            };
            var item2 = new Item
            {
                CampaignId = campaign.Id,
                Name = "Silver Amulet",
                Description = "An amulet bearing holy symbols.",
                Type = "Jewelry",
                Value = 150
            };
            db.Items.AddRange(item1, item2);
            await db.SaveChangesAsync();
            
            // =========
            // LOCATIONS
            var location1 = new Location
            {
                CampaignId = campaign.Id,
                Name = "Forgotten Crypt",
                Region = "Eastern Hills",
                Description = "Ancient tomb complex beneath cracked stone."
            };
            var location2 = new Location
            {
                CampaignId = campaign.Id,
                Name = "Black Hollow Camps",
                Region = "Darkwood Forest",
                Description = "Wooden camps scattered through thick trees."
            };
            db.Locations.AddRange(location1, location2);
            await db.SaveChangesAsync();
            
            // ====
            // NPCs
            var npc1 = new Npc
            {
                CampaignId = campaign.Id,
                Name = "Dorn Blackshield",
                Description = "Ex-mercenary guarding the crypt entrance.",
                RaceId = 1,
                ClassId = 5,
                Level = 4,
                Strength = 15,
                Dexterity = 13,
                Constitution = 14,
                Intelligence = 10,
                Wisdom = 11,
                Charisma = 9,
            };
            var npc2 = new Npc
            {
                CampaignId = campaign.Id,
                Name = "Lira Whisperwind",
                Description = "Bandit scout skilled in stealth.",
                RaceId = 2,
                ClassId = 9,
                Level = 3,
                Strength = 10,
                Dexterity = 16,
                Constitution = 12,
                Intelligence = 13,
                Wisdom = 12,
                Charisma = 14,
            };
            db.Npcs.AddRange(npc1, npc2);
            await db.SaveChangesAsync();
            
            // ==========
            // NPC SKILLS
            db.NpcSkills.AddRange(
                new NpcSkill
                {
                    NpcId = npc1.Id,
                    Name = "Intimidation",
                    Description = "Uses size and scars to frighten opponents.",
                    Value = 3
                },
                new NpcSkill
                {
                    NpcId = npc1.Id,
                    Name = "Athletics",
                    Description = "Strong combat training.",
                    Value = 4
                },

                new NpcSkill
                {
                    NpcId = npc2.Id,
                    Name = "Stealth",
                    Description = "Expert in moving unseen through forest.",
                    Value = 4
                },
                new NpcSkill
                {
                    NpcId = npc2.Id,
                    Name = "Perception",
                    Description = "Sharp eyes to scout victims.",
                    Value = 3
                }
            );
            await db.SaveChangesAsync();
            
            // ===========================
            // LINK EVERYTHING TO CHAPTERS
            db.ScenarioChapterNotes.AddRange(
                new ScenarioChapterNote { ScenarioChapterId = chapter1.Id, NoteId = note1.Id },
                new ScenarioChapterNote { ScenarioChapterId = chapter3.Id, NoteId = note2.Id }
            );
            db.ScenarioChapterItems.AddRange(
                new ScenarioChapterItem { ScenarioChapterId = chapter1.Id, ItemId = item1.Id },
                new ScenarioChapterItem { ScenarioChapterId = chapter2.Id, ItemId = item2.Id }
            );
            db.ScenarioChapterLocations.AddRange(
                new ScenarioChapterLocation { ScenarioChapterId = chapter1.Id, LocationId = location1.Id },
                new ScenarioChapterLocation { ScenarioChapterId = chapter3.Id, LocationId = location2.Id }
            );
            db.ScenarioChapterNpcs.AddRange(
                new ScenarioChapterNpc { ScenarioChapterId = chapter1.Id, NpcId = npc1.Id },
                new ScenarioChapterNpc { ScenarioChapterId = chapter3.Id, NpcId = npc2.Id }
            );
            await db.SaveChangesAsync();
        }

        await RuleSeeder.SeedRules(db);
    }
}
