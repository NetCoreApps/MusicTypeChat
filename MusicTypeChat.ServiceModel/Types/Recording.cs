using ServiceStack;
using ServiceStack.DataAnnotations;

namespace MusicTypeChat.ServiceModel.Types;

[Icon(Svg = Icons.Recording)]
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