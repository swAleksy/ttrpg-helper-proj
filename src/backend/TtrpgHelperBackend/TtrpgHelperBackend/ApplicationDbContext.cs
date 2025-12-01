using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Models;
using TtrpgHelperBackend.Models.Authentication;
using TtrpgHelperBackend.Models.Resource;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    // ==========
    // -- AUTH --
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    // ================
    // -- CHARACTERS --
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterSkill> CharacterSkills { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Background> Backgrounds { get; set; }
    
    // =============
    // -- SESSION --
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<SessionPlayer> SessionPlayers { get; set; }
    public DbSet<SessionEvent> SessionEvents { get; set; }
    
    // ==============
    // -- RESOURCE --
    public DbSet<Scenario> Scenarios { get; set; }
    public DbSet<ScenarioChapter> ScenarioChapters { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Npc> Npcs { get; set; }
    public DbSet<NpcSkill> NpcSkills { get; set; }
    public DbSet<ScenarioChapterNpc> ScenarioChapterNpcs { get; set; }
    public DbSet<ScenarioChapterItem> ScenarioChapterItems { get; set; }
    public DbSet<ScenarioChapterLocation> ScenarioChapterLocations { get; set; }
    public DbSet<ScenarioChapterNote> ScenarioChapterNotes { get; set; }
    
    // ==========================
    // -- CHAT & NOTIFICATIONS --
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
        
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
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
