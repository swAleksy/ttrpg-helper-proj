namespace TtrpgHelperBackend.Models.Resource;

public class ScenarioChapterItem
{
    public int ScenarioChapterId { get; set; }
    public ScenarioChapter ScenarioChapter { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
}