using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TtrpgHelperBackend.Models.Calendar;

namespace TtrpgHelperBackend.Configurations;

// ====================
// -- CALENDAR EVENT --
public class CalendarConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> entity)
    {
        entity.ToTable("CalendarEvents");
        
        entity.Property(e => e.Title)
            .IsRequired().
            HasMaxLength(50);
        
        entity.Property(e => e.Description)
            .HasMaxLength(200);
        
        entity.Property(e => e.EventDate)
            .IsRequired()
            .HasColumnType("timestamp without time zone");
        
        entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
