using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Configurations;

// ==============
// -- SCENARIO --
public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> entity)
    {
        entity.ToTable("Scenarios");

        entity.Property(s => s.Name)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(s => s.Description)
            .HasMaxLength(200);

        entity.HasOne(s => s.Campaign)
            .WithMany(c => c.Scenarios)
            .HasForeignKey(s => s.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(s => s.Chapters)
            .WithOne(c => c.Scenario)
            .HasForeignKey(c => c.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ======================
// -- SCENARIO CHAPTER --
public class ScenarioChapterConfiguration : IEntityTypeConfiguration<ScenarioChapter>
{
    public void Configure(EntityTypeBuilder<ScenarioChapter> entity)
    {
        entity.ToTable("ScenarioChapters");

        entity.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(c => c.ContentMarkdown);
    }
}

// ==========
// -- NOTE --
public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> entity)
    {
        entity.ToTable("Notes");

        entity.Property(n => n.Name)
            .HasMaxLength(50)
            .IsRequired();

        entity.HasOne(n => n.Campaign)
            .WithMany()
            .HasForeignKey(n => n.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ==========
// -- ITEM --
public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> entity)
    {
        entity.ToTable("Items");

        entity.Property(i => i.Name)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(i => i.Type)
            .HasMaxLength(30);

        entity.HasOne(i => i.Campaign)
            .WithMany()
            .HasForeignKey(i => i.CampaignId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}

// ==============
// -- LOCATION --
public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> entity)
    {
        entity.ToTable("Locations");

        entity.Property(l => l.Name)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(l => l.Region)
            .HasMaxLength(50);

        entity.HasOne(l => l.Campaign)
            .WithMany()
            .HasForeignKey(l => l.CampaignId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}

// =========
// -- NPC --
public class NpcConfiguration : IEntityTypeConfiguration<Npc>
{
    public void Configure(EntityTypeBuilder<Npc> entity)
    {
        entity.ToTable("Npcs");

        entity.Property(n => n.Name)
            .HasMaxLength(50)
            .IsRequired();

        entity.HasOne(n => n.Campaign)
            .WithMany()
            .HasForeignKey(n => n.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(n => n.Race)
            .WithMany()
            .HasForeignKey(n => n.RaceId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(n => n.Class)
            .WithMany()
            .HasForeignKey(n => n.ClassId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}

// ===============
// -- NPC SKILL --
public class NpcSkillConfiguration : IEntityTypeConfiguration<NpcSkill>
{
    public void Configure(EntityTypeBuilder<NpcSkill> entity)
    {
        entity.ToTable("NpcSkills");

        entity.Property(s => s.Name)
            .HasMaxLength(50)
            .IsRequired();

        entity.HasOne(s => s.Npc)
            .WithMany(n => n.Skills)
            .HasForeignKey(s => s.NpcId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ===============================
// -- SCENARIO CHAPTER <-> NOTE --
public class ScenarioChapterNoteConfiguration : IEntityTypeConfiguration<ScenarioChapterNote>
{
    public void Configure(EntityTypeBuilder<ScenarioChapterNote> entity)
    {
        entity.ToTable("ScenarioChapterNotes");

        entity.HasKey(x => new { x.ScenarioChapterId, x.NoteId });

        entity.HasOne(x => x.ScenarioChapter)
            .WithMany(c => c.Notes)
            .HasForeignKey(x => x.ScenarioChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Note)
            .WithMany(n => n.ChapterLinks)
            .HasForeignKey(x => x.NoteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ==============================
// -- SCENARIO CHAPTER <-> NPC --
public class ScenarioChapterNpcConfiguration : IEntityTypeConfiguration<ScenarioChapterNpc>
{
    public void Configure(EntityTypeBuilder<ScenarioChapterNpc> entity)
    {
        entity.ToTable("ScenarioChapterNpcs");

        entity.HasKey(x => new { x.ScenarioChapterId, x.NpcId });

        entity.HasOne(x => x.ScenarioChapter)
            .WithMany(c => c.Npcs)
            .HasForeignKey(x => x.ScenarioChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Npc)
            .WithMany(n => n.ChapterLinks)
            .HasForeignKey(x => x.NpcId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ===============================
// -- SCENARIO CHAPTER <-> ITEM --
public class ScenarioChapterItemConfiguration : IEntityTypeConfiguration<ScenarioChapterItem>
{
    public void Configure(EntityTypeBuilder<ScenarioChapterItem> entity)
    {
        entity.ToTable("ScenarioChapterItems");

        entity.HasKey(x => new { x.ScenarioChapterId, x.ItemId });

        entity.HasOne(x => x.ScenarioChapter)
            .WithMany(c => c.Items)
            .HasForeignKey(x => x.ScenarioChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Item)
            .WithMany(i => i.ChapterLinks)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ===================================
// -- SCENARIO CHAPTER <-> LOCATION --
public class ScenarioChapterLocationConfiguration : IEntityTypeConfiguration<ScenarioChapterLocation>
{
    public void Configure(EntityTypeBuilder<ScenarioChapterLocation> entity)
    {
        entity.ToTable("ScenarioChapterLocations");

        entity.HasKey(x => new { x.ScenarioChapterId, x.LocationId });

        entity.HasOne(x => x.ScenarioChapter)
            .WithMany(c => c.Locations)
            .HasForeignKey(x => x.ScenarioChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Location)
            .WithMany(l => l.ChapterLinks)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ==========
// -- RULE --
public class RuleConfiguration : IEntityTypeConfiguration<Rule>
{
    public void Configure(EntityTypeBuilder<Rule> entity)
    {
        entity.ToTable("Rules");

        entity.Property(r => r.Category)
            .HasMaxLength(30);
        
        entity.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        entity.Property(r => r.ContentMarkdown)
            .IsRequired();
    }
}
