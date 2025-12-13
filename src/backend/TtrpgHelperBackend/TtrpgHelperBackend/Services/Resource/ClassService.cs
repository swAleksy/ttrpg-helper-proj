using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource.Class;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IClassService
{
    Task<GetClassDto?> GetClass(int classId);
    Task<IEnumerable<GetClassDto>> GetClasses();
    Task<GetClassDto?> CreateClass(CreateClassDto dto);
    Task<GetClassDto?> UpdateClass(UpdateClassDto dto);
    Task<bool> DeleteClass(int classId);
}

public class ClassService
{
    private readonly ApplicationDbContext _db;

    public ClassService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetClassDto?> GetClass(int classId)
    {
        var cls = await _db.Classes
            .FirstOrDefaultAsync(b => b.Id == classId);
        if (cls == null) return null;
        
        return Dto(cls);
    }

    public async Task<IEnumerable<GetClassDto>> GetClasses()
    {
        var classes = await _db.Classes
            .ToListAsync();
        
        return classes.Select(Dto).ToList();
    }

    public async Task<GetClassDto?> CreateClass(CreateClassDto dto)
    {
        var newClass = new Class
        {
            Name = dto.Name,
            Description = dto.Description,
        };
        
        _db.Classes.Add(newClass);
        await _db.SaveChangesAsync();
        
        return Dto(newClass);
    }

    public async Task<GetClassDto?> UpdateClass(UpdateClassDto dto)
    {
        var updated = await _db.Classes
            .FirstOrDefaultAsync(b => b.Id == dto.Id);
        if (updated == null) return null;

        updated.Name = dto.Name;
        updated.Description = dto.Description;
        
        await _db.SaveChangesAsync();
        
        return Dto(updated);
    }

    public async Task<bool> DeleteClass(int classId)
    {
        var deleted = await _db.Classes
            .FirstOrDefaultAsync(b => b.Id == classId);
        if (deleted == null) return false;
        
        _db.Classes.Remove(deleted);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private static GetClassDto Dto(Class cls)
    {
        return new GetClassDto
        {
            Id = cls.Id,
            Name = cls.Name,
            Description = cls.Description,
        };
    }
}
