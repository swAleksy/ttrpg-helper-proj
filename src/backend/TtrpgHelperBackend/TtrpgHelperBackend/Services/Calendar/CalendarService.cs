using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Calendar;
using TtrpgHelperBackend.Models.Calendar;

namespace TtrpgHelperBackend.Services.Calendar;

public interface ICalendarService
{
    Task<GetCalendarEventDto?> GetEvent(int eventId, int userId);
    Task<IEnumerable<GetCalendarDto>> GetCalendar(int userId);
    Task<GetCalendarEventDto> CreateEvent(CreateCalendarEventDto dto, int userId);
    Task<GetCalendarEventDto?> UpdateEvent(UpdateCalendarEventDto dto, int userId);
    Task<bool> DeleteEvent(int eventId, int userId);
}

public class CalendarService : ICalendarService
{
    private readonly ApplicationDbContext _db;

    public CalendarService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<GetCalendarEventDto?> GetEvent(int eventId, int userId) 
    {
        var calendarEvent = await _db.CalendarEvents
            .FirstOrDefaultAsync(e => e.Id == eventId && e.UserId == userId);
        if (calendarEvent == null) return null;

        return Dto(calendarEvent);
    }

    public async Task<IEnumerable<GetCalendarDto>> GetCalendar(int userId)
    {
        var sessions = await _db.Sessions
            .Include(s => s.Players)
            .Include(s => s.Campaign)
            .Where(s => s.Campaign.GameMasterId == userId || s.Players.Any(p => p.PlayerId == userId))
            .Select(s => new GetCalendarDto
            {
                Id = s.Id,
                Type = "session",
                Title = s.Name,
                Description = s.Description ?? "",
                Date = s.ScheduledDate,
                CampaignId = s.CampaignId,
                SessionId = s.Id
            })
            .ToListAsync();
        
        var calendarEvents = await _db.CalendarEvents
            .Where(e => e.UserId == userId)
            .Select(e => new GetCalendarDto
            {
                Id = e.Id,
                Type = "event",
                Title = e.Title,
                Description = e.Description,
                Date = e.EventDate,
                CampaignId = null,
                SessionId = null
            })
            .ToListAsync();
        
        return sessions
            .Concat(calendarEvents)
            .OrderBy(e => e.Date)
            .ToList();
    }
    
    public async Task<GetCalendarEventDto> CreateEvent(CreateCalendarEventDto dto, int userId) 
    {
        var calendarEvent = new CalendarEvent
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            EventDate = dto.EventDate
        };

        _db.CalendarEvents.Add(calendarEvent);
        await _db.SaveChangesAsync();
        
        return Dto(calendarEvent);
    }
    
    public async Task<GetCalendarEventDto?> UpdateEvent(UpdateCalendarEventDto dto, int userId) 
    {
        var calendarEvent = await _db.CalendarEvents
            .FirstOrDefaultAsync(e => e.Id == dto.Id && e.UserId == userId);

        if (calendarEvent == null)
            return null;

        calendarEvent.Title = dto.Title;
        calendarEvent.Description = dto.Description;
        calendarEvent.EventDate = dto.EventDate;

        await _db.SaveChangesAsync();

        return Dto(calendarEvent);
    }
    
    public async Task<bool> DeleteEvent(int eventId, int userId) 
    {
        var calendarEvent = await _db.CalendarEvents
            .FirstOrDefaultAsync(e => e.Id == eventId && e.UserId == userId);

        if (calendarEvent == null) return false;

        _db.CalendarEvents.Remove(calendarEvent);
        await _db.SaveChangesAsync();

        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private static GetCalendarEventDto Dto(CalendarEvent calendarEvent)
    {
        return new GetCalendarEventDto
        {
            Id = calendarEvent.Id,
            UserId = calendarEvent.UserId,
            Title = calendarEvent.Title,
            Description = calendarEvent.Description,
            EventDate = calendarEvent.EventDate
        };
    }
}
