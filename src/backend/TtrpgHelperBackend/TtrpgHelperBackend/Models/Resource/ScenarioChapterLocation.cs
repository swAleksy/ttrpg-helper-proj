namespace TtrpgHelperBackend.Models.Resource;

public class ScenarioChapterLocation
{
    public int ScenarioChapterId { get; set; }
    public ScenarioChapter ScenarioChapter { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
}