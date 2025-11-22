namespace TtrpgHelperBackend.DTOs.Resource;

public class GetScenarioChapterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContentMarkdown { get; set; } = string.Empty;

    public IEnumerable<GetScenarioNoteDto> Notes { get; set; } = new List<GetScenarioNoteDto>();
    public IEnumerable<GetScenarioNpcDto> Npcs { get; set; } = new List<GetScenarioNpcDto>();
    public IEnumerable<GetScenarioItemDto> Items { get; set; } = new List<GetScenarioItemDto>();
    public IEnumerable<GetScenarioLocationDto> Locations { get; set; } = new List<GetScenarioLocationDto>();
}