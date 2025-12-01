using TtrpgHelperBackend.Models;

namespace TtrpgHelperBackend.Seed;

public class CharacterSeeder
{
    public static async Task SeedCharacters(ApplicationDbContext db)
    {
        if (!db.Races.Any() && !db.Classes.Any() && !db.Skills.Any() && !db.Backgrounds.Any() && !db.Characters.Any())
        {
            await SRaces(db);
            await SClasses(db);
            await SSkills(db);
            await SBackgrounds(db);
            await SCharacters(db);
        }
    }

    
    private static async Task SRaces(ApplicationDbContext db)
    {
        db.Races.AddRange(
            new Race { Name = "Human",      Description = "Versatile and adaptable, humans receive a bonus to all abilities." },
            new Race { Name = "Elf",        Description = "Graceful and long-lived, elves are dexterous and attuned to magic." },
            new Race { Name = "Dwarf",      Description = "Stout and hardy, dwarves are resilient in combat and skilled craftsmen." },
            new Race { Name = "Halfling",   Description = "Small and nimble, halflings are lucky and quick in tricky situations." },
            new Race { Name = "Half-Elf",   Description = "A blend of human and elven heritage, charismatic and versatile." },
            new Race { Name = "Half-Orc",   Description = "Strong and ferocious, half-orcs have orcish blood and fierce instincts." },
            new Race { Name = "Gnome",      Description = "Small in stature and quick of mind, gnomes excel in intelligence and cunning." },
            new Race { Name = "Tiefling",   Description = "Marked by infernal heritage, tieflings wield otherworldly power and charisma." },
            new Race { Name = "Drow",       Description = "Dark elves of the Underdark, with keen senses and shadow-affinities." },
            new Race { Name = "Githyanki",  Description = "Warrior-bred astral-plane beings, fierce in combat and psionically gifted." },
            new Race { Name = "Dragonborn", Description = "Draconic-bodied humanoids, born of dragon-ancestors, with breath weapons." }
        );
        
        await db.SaveChangesAsync();
    }

    private static async Task SClasses(ApplicationDbContext db)
    {
        db.Classes.AddRange(
            new Class { Name = "Barbarian",   Description = "A fierce warrior of primal strength and rage." },
            new Class { Name = "Bard",        Description = "A charismatic performer and jack-of-all-trades using song and magic." },
            new Class { Name = "Cleric",      Description = "A divine spellcaster and healer, empowered by a deity or faith." },
            new Class { Name = "Druid",       Description = "A master of nature, able to shapeshift and call upon natural powers." },
            new Class { Name = "Fighter",     Description = "A skilled and versatile warrior trained in weapons and armour." },
            new Class { Name = "Monk",        Description = "A martial artist using ki, speed, and precision in combat." },
            new Class { Name = "Paladin",     Description = "A holy warrior bound by oath, wielding divine power and martial might." },
            new Class { Name = "Ranger",      Description = "A wilderness scout, expert with ranged weapons and nature’s allies." },
            new Class { Name = "Rogue",       Description = "A stealthy opportunist, skilled in infiltration, tricks and precision attacks." },
            new Class { Name = "Sorcerer",    Description = "A spontaneous arcane caster whose magic comes from innate power." },
            new Class { Name = "Warlock",     Description = "A spellcaster who has made a pact with a powerful entity." },
            new Class { Name = "Wizard",      Description = "A studious arcane caster whose power comes from rigorous training and knowledge." }
        );

        await db.SaveChangesAsync();
    }

    private static async Task SSkills(ApplicationDbContext db)
    {
        db.Skills.AddRange(
            new Skill { Name = "Acrobatics",        Description = "Balance, tumble, avoid falling or being shoved." },
            new Skill { Name = "Animal Handling",   Description = "Interact, calm, or command animals." },
            new Skill { Name = "Arcana",            Description = "Knowledge of magic, magical effects and items." },
            new Skill { Name = "Athletics",         Description = "Climb, swim, jump, and physically struggle." },
            new Skill { Name = "Deception",         Description = "Lie convincingly, deceive others." },
            new Skill { Name = "History",           Description = "Recall lore about past events, places, people." },
            new Skill { Name = "Insight",           Description = "Sense motives, detect lies, read people." },
            new Skill { Name = "Intimidation",      Description = "Coerce or bully others through fear or strength." },
            new Skill { Name = "Investigation",     Description = "Examine, search, deduce hidden clues and details." },
            new Skill { Name = "Medicine",          Description = "Treat injuries, diagnose illness, apply healing." },
            new Skill { Name = "Nature",            Description = "Understand flora, fauna, natural environment." },
            new Skill { Name = "Perception",        Description = "Notice hidden things, traps, secret doors, distant sounds." },
            new Skill { Name = "Performance",       Description = "Entertain or impress through music, acting, or oration." },
            new Skill { Name = "Persuasion",        Description = "Convince or influence others socially." },
            new Skill { Name = "Religion",          Description = "Knowledge of deities, religious rites, sacred things." },
            new Skill { Name = "Sleight of Hand",   Description = "Pickpocket, manipulate objects subtly, perform tricks." },
            new Skill { Name = "Stealth",           Description = "Move silently, hide, sneak past detection." },
            new Skill { Name = "Survival",          Description = "Track, forage, endure wilderness, navigate terrain." }
        );
        
        await db.SaveChangesAsync();
    }

    private static async Task SBackgrounds(ApplicationDbContext db)
    {
        db.Backgrounds.AddRange(
            new Background 
            { 
                Name = "Soldier",
                Description = "Former member of a standing army or mercenary company. Trained in discipline and battlefield tactics." 
            },
            new Background 
            { 
                Name = "Noble",
                Description = "Raised among the aristocracy, familiar with politics, etiquette and influence." 
            },
            new Background 
            { 
                Name = "Criminal",
                Description = "Spent years among thieves, smugglers or gangs. Knows how the underworld works." 
            },
            new Background 
            { 
                Name = "Scholar",
                Description = "Dedicated to academic study, lore research and ancient knowledge." 
            },
            new Background 
            { 
                Name = "Outlander",
                Description = "Grew up in the wilderness, skilled in survival and navigation far from civilization." 
            },
            new Background 
            { 
                Name = "Acolyte",
                Description = "Served in a religious order or temple, learning rituals, doctrine and ecclesiastical duties." 
            },
            new Background 
            { 
                Name = "Sailor",
                Description = "Spent years at sea aboard merchant or naval vessels. Knows ports, tides and hard work." 
            },
            new Background
            {
                Name = "Entertainer",
                Description = "Traveled the lands performing music, acting or storytelling to earn a living."
            }
        );
        
        await db.SaveChangesAsync();
    }

    private static async Task SCharacters(ApplicationDbContext db)
    {
        var raceHuman = db.Races.First(r => r.Name == "Human");
        var raceElf = db.Races.First(r => r.Name == "Elf");
        var raceDwarf = db.Races.First(r => r.Name == "Dwarf");

        var fighter = db.Classes.First(c => c.Name == "Fighter");
        var wizard = db.Classes.First(c => c.Name == "Wizard");
        var rogue = db.Classes.First(c => c.Name == "Rogue");

        var bgSoldier = db.Backgrounds.First(b => b.Name == "Soldier");
        var bgScholar = db.Backgrounds.First(b => b.Name == "Scholar");
        var bgCriminal = db.Backgrounds.First(b => b.Name == "Criminal");
        
        var c1 = new Character
        {
            UserId = 3,
            Name = "Thorian Ironfist",
            RaceId = raceDwarf.Id,
            ClassId = fighter.Id,
            BackgroundId = bgSoldier.Id,
            Level = 3,
            Strength = 16,
            Dexterity = 12,
            Constitution = 15,
            Intelligence = 9,
            Wisdom = 11,
            Charisma = 10
        };

        var c2 = new Character
        {
            UserId = 4,
            Name = "Elira Moonwhisper",
            RaceId = raceElf.Id,
            ClassId = wizard.Id,
            BackgroundId = bgScholar.Id,
            Level = 2,
            Strength = 8,
            Dexterity = 14,
            Constitution = 10,
            Intelligence = 16,
            Wisdom = 13,
            Charisma = 12
        };

        var c3 = new Character
        {
            UserId = 5,
            Name = "Kara Shadowstep",
            RaceId = raceHuman.Id,
            ClassId = rogue.Id,
            BackgroundId = bgCriminal.Id,
            Level = 3,
            Strength = 10,
            Dexterity = 16,
            Constitution = 12,
            Intelligence = 13,
            Wisdom = 11,
            Charisma = 14
        };
        db.Characters.AddRange(c1, c2, c3);
        await db.SaveChangesAsync();
        
        var skills = db.Skills.ToList();
        void AddSkill(Character c, string skillName)
        {
            var skill = skills.First(s => s.Name == skillName);

            db.CharacterSkills.Add(new CharacterSkill
            {
                CharacterId = c.Id,
                SkillId = skill.Id
            });
        }
        
        AddSkill(c1, "Athletics");
        AddSkill(c1, "Intimidation");
        AddSkill(c1, "Survival");
        
        AddSkill(c2, "Arcana");
        AddSkill(c2, "History");
        AddSkill(c2, "Investigation");
        
        AddSkill(c3, "Stealth");
        AddSkill(c3, "Sleight of Hand");
        AddSkill(c3, "Deception");
        AddSkill(c3, "Acrobatics");
        
        await db.SaveChangesAsync();
    }
}