using ServiceStack;
using ServiceStack.Gpt;
using ServiceStack.Script;

namespace MusicTypeChat.ServiceInterface;

public class MusicPromptProvider : IPromptProvider
{
    public AppConfig Config { get; set; }

    public MusicPromptProvider(AppConfig config)
    {
        Config = config;
    }

    public async Task<string> CreateSchemaAsync(CancellationToken token = default)
    {
        var file = new FileInfo(Config.SiteConfig.GptPath.CombineWith("schema.ss"));
        if (file == null)
            throw HttpError.NotFound($"{Config.SiteConfig.GptPath}/schema.ss not found");
        
        var tpl = await file.ReadAllTextAsync(token: token);
        var context = new ScriptContext {
            Plugins = { new TypeScriptPlugin() }
        }.Init();

        var output = await new PageResult(context.OneTimePage(tpl))
        {
            Args = new Dictionary<string, object>(),
        }.RenderScriptAsync(token: token);
        return output;
    }

    public async Task<string> CreatePromptAsync(string userMessage, CancellationToken token = default)
    {
        var file = new FileInfo(Config.SiteConfig.GptPath.CombineWith("prompt.ss"));
        if (file == null)
            throw HttpError.NotFound($"{Config.SiteConfig.GptPath}/prompt.ss not found");
        
        var schema = await CreateSchemaAsync(token:token);
        var tpl = await file.ReadAllTextAsync(token: token);
        var context = new ScriptContext {
            Plugins = { new TypeScriptPlugin() }
        }.Init();

        var prompt = await new PageResult(context.OneTimePage(tpl))
        {
            Args =
            {
                [nameof(schema)] = schema,
                [nameof(userMessage)] = userMessage,
            }
        }.RenderScriptAsync(token: token);

        return prompt;
    }
}

public class AppConfig
{
    public string Project { get; set; }
    public string Location { get; set; }
    public SiteConfig SiteConfig { get; set; }
    public string NodePath { get; set; }
    public string? FfmpegPath { get; set; }
    public string? WhisperPath { get; set; }
    public int NodeProcessTimeoutMs { get; set; } = 120 * 1000;

    public GoogleCloudSpeechConfig GoogleCloudSpeechConfig() => new()
    {
        Project = Project,
        Location = Location,
        Bucket = SiteConfig.Bucket,
        RecognizerId = SiteConfig.RecognizerId,
        PhraseSetId = SiteConfig.PhraseSetId,
    };
}

public class SiteConfig
{
    public string GptPath { get; set; }
    public string Bucket { get; set; }
    public string RecognizerId { get; set; }
    public string PhraseSetId { get; set; }
}
