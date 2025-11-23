using TtrpgHelperBackend.DTOs.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IScenarioService
{
    Task<GetScenarioDto?> GetScenario(int id, int gameMasterId);
    Task<IEnumerable<GetScenarioDto>> GetScenarios(int campaignId, int gameMasterId);
}

public class ScenarioService  : IScenarioService
{
    private readonly ApplicationDbContext _db;

    public ScenarioService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    
}