using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace MusicTypeChat.Migrations;

public class Recording
{
    [AutoIncrement]
    public int Id { get; set; }
    public string Path { get; set; }
    public string? Transcript { get; set; }
    public float? TranscriptConfidence { get; set; }
    public string? TranscriptResponse { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? TranscribeStart { get; set; }
    public DateTime? TranscribeEnd { get; set; }
    public int? TranscribeDurationMs { get; set; }
    public int? DurationMs { get; set; }
    public string? IpAddress { get; set; }
    public string? Error { get; set; }
}

public class Migration1001 : MigrationBase
{
    public override void Up()
    {
        Db.CreateTable<Recording>();
    }
    
    public override void Down()
    {
        Db.DropTable<Recording>();
    }
}