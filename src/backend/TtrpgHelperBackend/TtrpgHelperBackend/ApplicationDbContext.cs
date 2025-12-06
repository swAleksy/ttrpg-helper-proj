using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Models;
using TtrpgHelperBackend.Models.Authentication;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterSkill> CharacterSkills { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Background> Backgrounds { get; set; }
    
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<SessionPlayer> SessionPlayers { get; set; }
    public DbSet<SessionEvent> SessionEvents { get; set; }
    
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" }
        );
        
        modelBuilder.Entity<CharacterSkill>()
            .HasKey(cs => new { cs.CharacterId, cs.SkillId });

        // Define the relationship from the join table to Character
        modelBuilder.Entity<CharacterSkill>()
            .HasOne(cs => cs.Character)
            .WithMany(c => c.CharacterSkills)
            .HasForeignKey(cs => cs.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Define the relationship from the join table to Skill
        modelBuilder.Entity<CharacterSkill>()
            .HasOne(cs => cs.Skill)
            .WithMany(s => s.CharacterSkills)
            .HasForeignKey(cs => cs.SkillId);
        
        // -- SESSION MODULE --
        
        // -- SESSION --
        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("Sessions");

            entity.Property(s => s.ScheduledDate)
                .HasColumnType("timestamp without time zone");

            entity.Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(s => s.Description)
                .HasMaxLength(200);

            entity.Property(s => s.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasOne(s => s.Campaign)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Players)
                .WithOne(sp => sp.Session)
                .HasForeignKey(sp => sp.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Events)
                .WithOne(se => se.Session)
                .HasForeignKey(se => se.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // -- CAMPAIGN --
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.ToTable("Campaigns");

            entity.Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(c => c.Description)
                .HasMaxLength(1000);

            entity.Property(c => c.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasOne(c => c.GameMaster)
                .WithMany()
                .HasForeignKey(c => c.GameMasterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.Sessions)
                .WithOne(s => s.Campaign)
                .HasForeignKey(s => s.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // -- SESSION PLAYER --
        modelBuilder.Entity<SessionPlayer>(entity =>
        {
            entity.ToTable("SessionPlayers");

            entity.HasOne(sp => sp.Session)
                .WithMany(s => s.Players)
                .HasForeignKey(sp => sp.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sp => sp.Player)
                .WithMany()
                .HasForeignKey(sp => sp.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(sp => new { sp.SessionId, sp.PlayerId })
                .IsUnique();
        });
        
        // -- SESSION EVENT --
        modelBuilder.Entity<SessionEvent>(entity =>
        {
            entity.ToTable("SessionEvents");

            entity.HasOne(e => e.Session)
                .WithMany(s => s.Events)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .IsRequired();
        });
        
        // -- SESSION MODULE --
        
        modelBuilder.Entity<Class>().HasData(
            new Class { Id = 1,  Name = "Barbarian",   Description = "A fierce warrior of primal strength and rage." },
            new Class { Id = 2,  Name = "Bard",        Description = "A charismatic performer and jack-of-all-trades using song and magic." },
            new Class { Id = 3,  Name = "Cleric",      Description = "A divine spellcaster and healer, empowered by a deity or faith." },
            new Class { Id = 4,  Name = "Druid",       Description = "A master of nature, able to shapeshift and call upon natural powers." },
            new Class { Id = 5,  Name = "Fighter",     Description = "A skilled and versatile warrior trained in weapons and armour." },
            new Class { Id = 6,  Name = "Monk",        Description = "A martial artist using ki, speed, and precision in combat." },
            new Class { Id = 7,  Name = "Paladin",     Description = "A holy warrior bound by oath, wielding divine power and martial might." },
            new Class { Id = 8,  Name = "Ranger",      Description = "A wilderness scout, expert with ranged weapons and nature’s allies." },
            new Class { Id = 9,  Name = "Rogue",       Description = "A stealthy opportunist, skilled in infiltration, tricks and precision attacks." },
            new Class { Id = 10, Name = "Sorcerer",    Description = "A spontaneous arcane caster whose magic comes from innate power." },
            new Class { Id = 11, Name = "Warlock",     Description = "A spellcaster who has made a pact with a powerful entity." },
            new Class { Id = 12, Name = "Wizard",      Description = "A studious arcane caster whose power comes from rigorous training and knowledge." }
        );
        
        modelBuilder.Entity<Race>().HasData(
            new Race { Id = 1,  Name = "Human",      Description = "Versatile and adaptable, humans receive a bonus to all abilities." },
            new Race { Id = 2,  Name = "Elf",        Description = "Graceful and long-lived, elves are dexterous and attuned to magic." },
            new Race { Id = 3,  Name = "Dwarf",      Description = "Stout and hardy, dwarves are resilient in combat and skilled craftsmen." },
            new Race { Id = 4,  Name = "Halfling",   Description = "Small and nimble, halflings are lucky and quick in tricky situations." },
            new Race { Id = 5,  Name = "Half-Elf",   Description = "A blend of human and elven heritage, charismatic and versatile." },
            new Race { Id = 6,  Name = "Half-Orc",   Description = "Strong and ferocious, half-orcs have orcish blood and fierce instincts." },
            new Race { Id = 7,  Name = "Gnome",      Description = "Small in stature and quick of mind, gnomes excel in intelligence and cunning." },
            new Race { Id = 8,  Name = "Tiefling",   Description = "Marked by infernal heritage, tieflings wield otherworldly power and charisma." },
            new Race { Id = 9,  Name = "Drow",       Description = "Dark elves of the Underdark, with keen senses and shadow-affinities." },
            new Race { Id = 10, Name = "Githyanki",  Description = "Warrior-bred astral-plane beings, fierce in combat and psionically gifted." },
            new Race { Id = 11, Name = "Dragonborn", Description = "Draconic-bodied humanoids, born of dragon-ancestors, with breath weapons." }
        );
        
        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = 1,  Name = "Acrobatics",        Description = "Balance, tumble, avoid falling or being shoved." },
            new Skill { Id = 2,  Name = "Animal Handling",   Description = "Interact, calm, or command animals." },
            new Skill { Id = 3,  Name = "Arcana",            Description = "Knowledge of magic, magical effects and items." },
            new Skill { Id = 4,  Name = "Athletics",         Description = "Climb, swim, jump, and physically struggle." },
            new Skill { Id = 5,  Name = "Deception",         Description = "Lie convincingly, deceive others." },
            new Skill { Id = 6,  Name = "History",           Description = "Recall lore about past events, places, people." },
            new Skill { Id = 7,  Name = "Insight",           Description = "Sense motives, detect lies, read people." },
            new Skill { Id = 8,  Name = "Intimidation",      Description = "Coerce or bully others through fear or strength." },
            new Skill { Id = 9,  Name = "Investigation",     Description = "Examine, search, deduce hidden clues and details." },
            new Skill { Id = 10, Name = "Medicine",          Description = "Treat injuries, diagnose illness, apply healing." },
            new Skill { Id = 11, Name = "Nature",            Description = "Understand flora, fauna, natural environment." },
            new Skill { Id = 12, Name = "Perception",        Description = "Notice hidden things, traps, secret doors, distant sounds." },
            new Skill { Id = 13, Name = "Performance",       Description = "Entertain or impress through music, acting, or oration." },
            new Skill { Id = 14, Name = "Persuasion",        Description = "Convince or influence others socially." },
            new Skill { Id = 15, Name = "Religion",          Description = "Knowledge of deities, religious rites, sacred things." },
            new Skill { Id = 16, Name = "Sleight of Hand",   Description = "Pickpocket, manipulate objects subtly, perform tricks." },
            new Skill { Id = 17, Name = "Stealth",           Description = "Move silently, hide, sneak past detection." },
            new Skill { Id = 18, Name = "Survival",          Description = "Track, forage, endure wilderness, navigate terrain." }
        );
        modelBuilder.Entity<Background>().HasData(
            new Background { Id = 1, Name = "Brakowalo tego :)", Description = "Dawno dawno temu w dupe" }
        );
    }
}