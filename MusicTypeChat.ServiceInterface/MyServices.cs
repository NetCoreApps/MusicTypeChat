using ServiceStack;
using MusicTypeChat.ServiceModel;
using ServiceStack.Gpt;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using SpotifyAPI.Web;

namespace MusicTypeChat.ServiceInterface;

public class MyServices : Service
{
    public object Any(Hello request)
    {
        return new HelloResponse { Result = $"Hello, {request.Name}!" };
    }
    
    public ISpeechToText SpeechToText { get; set; }
    public IAutoQueryDb AutoQuery { get; set; }
    
    public async Task<object> Any(TranscribeAudio request)
    {
        var recording = (Recording)await AutoQuery.CreateAsync(request, Request);

        var transcribeStart = DateTime.UtcNow;
        await Db.UpdateOnlyAsync(() => new Recording { TranscribeStart = transcribeStart },
            where: x => x.Id == recording.Id);

        try
        {
            var result = await SpeechToText.TranscribeAsync(request.Path);
            var transcribeEnd = DateTime.UtcNow;
            await Db.UpdateOnlyAsync(() => new Recording
            {
                Transcript = result.Transcript,
                TranscriptConfidence = result.Confidence,
                TranscriptResponse = result.ApiResponse,
                TranscribeEnd = transcribeEnd,
                TranscribeDurationMs = (int)(transcribeEnd - transcribeStart).TotalMilliseconds,
            }, where: x => x.Id == recording.Id);
        }
        catch (Exception e)
        {
            await Db.UpdateOnlyAsync(() => new Recording { Error = e.Message },
                where: x => x.Id == recording.Id);
        }

        recording = await Db.SingleByIdAsync<Recording>(recording.Id);

        WriteJsonFile($"/speech-to-text/{recording.CreatedDate:yyyy/MM/dd}/{recording.CreatedDate.TimeOfDay.TotalMilliseconds}.json", 
            recording.ToJson());

        return recording;
    }
    
    void WriteJsonFile(string path, string json)
    {
        ThreadPool.QueueUserWorkItem(_ => {
            try
            {
                VirtualFiles.WriteFile(path, json);
            }
            catch (Exception ignore) {}
        });
    }
}


