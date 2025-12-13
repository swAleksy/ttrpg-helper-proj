using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TtrpgHelperBackend.Models.Session;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.Configurations;

// ==============
// -- CAMPAIGN --
public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> entity)
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
    }
}

// =============
// -- SESSION --
public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> entity)
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
    }
}

// ====================
// -- SESSION PLAYER --
public class SessionPlayerConfiguration : IEntityTypeConfiguration<SessionPlayer>
{
    public void Configure(EntityTypeBuilder<SessionPlayer> entity)
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
    }
}

// ===================
// -- SESSION EVENT --
public class SessionEventConfiguration : IEntityTypeConfiguration<SessionEvent>
{
    public void Configure(EntityTypeBuilder<SessionEvent> entity)
    {
        entity.ToTable("SessionEvents");

        entity.Property(e => e.Type)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.Timestamp)
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        entity.HasOne(e => e.Session)
            .WithMany(s => s.Events)
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
