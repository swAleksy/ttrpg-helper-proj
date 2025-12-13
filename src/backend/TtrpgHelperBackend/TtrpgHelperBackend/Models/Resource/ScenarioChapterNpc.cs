namespace TtrpgHelperBackend.Models.Resource;

public class ScenarioChapterNpc
{
    public int ScenarioChapterId { get; set; }
    public ScenarioChapter ScenarioChapter { get; set; } = null!;

    public int NpcId { get; set; }
    public Npc Npc { get; set; } = null!;
}
