using Microsoft.EntityFrameworkCore;

namespace TtrpgHelperBackend;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
}