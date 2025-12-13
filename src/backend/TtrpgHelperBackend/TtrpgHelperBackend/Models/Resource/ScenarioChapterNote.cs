namespace TtrpgHelperBackend.Models.Resource;

public class ScenarioChapterNote
{
    public int ScenarioChapterId { get; set; }
    public ScenarioChapter ScenarioChapter { get; set; } = null!;

    public int NoteId { get; set; }
    public Note Note { get; set; } = null!;
}