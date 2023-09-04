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

    public async Task<object> Any(AdminData request)
    {
        var tables = new (string Label, Type Type)[] 
        {
            ("Bookings", typeof(Booking)),
            ("Coupons",  typeof(Coupon)),
        };
        var dialect = Db.GetDialectProvider();
        var totalSql = tables.Map(x => $"SELECT '{x.Label}', COUNT(*) FROM {dialect.GetQuotedTableName(x.Type.GetModelMetadata())}")
            .Join(" UNION ");
        var results = await Db.DictionaryAsync<string,int>(totalSql);
        
        return new AdminDataResponse {
            PageStats = tables.Map(x => new PageStats {
                Label = x.Label, 
                Total = results[x.Label],
            })
        };
    }
    
    public ITypeChatProvider TypeChatProvider { get; set; }
    public IPromptProvider PromptProvider { get; set; }
    public AppConfig Config { get; set; }
    
    public TypeChatRequest CreateTypeChatRequest(string userMessage) => new(PromptProvider, userMessage) {
        NodePath = Config.NodePath,
        NodeProcessTimeoutMs = Config.NodeProcessTimeoutMs,
        WorkingDirectory = Environment.CurrentDirectory,
        SchemaPath = Config.SiteConfig.GptPath.CombineWith("schema.ts"),
        TypeChatTranslator = TypeChatTranslator.Program
    };
    
 
    public async Task<object> Post(ProcessSpotifyCommand request)
    {
        var session = GetSession().GetAuthTokens("spotify");
        if(session == null)
            throw new Exception("Spotify session not found");
        var program =
            await TypeChatProvider.TranslateMessageAsync(CreateTypeChatRequest(request.UserMessage));

        var programRequest = program.Result.FromJson<TypeChatProgramResponse>();
        var programResult = await BindAndRun<SpotifyProgram>(programRequest, new Dictionary<string, string>
        {
            {"SpotifyToken",session.AccessToken}
        });
        return programResult;
    }
    
    private async Task<object> BindAndRun<T>(TypeChatProgramResponse mathResult, Dictionary<string, string>? config = null) where T : TypeChatProgramBase, new() 
    {
        var prog = new T { Config = config ?? new Dictionary<string, string>() };
        prog.Init();

        var steps = mathResult.Steps;
        object? result = null;
        foreach (var step in steps)
        {
            result = await ProcessStep(step, prog);
            prog.StepResults.Add(result);
        }

        return result;
    }
    
    private async Task<object> ProcessStep<T>(TypeChatStep step, T prog) where T : TypeChatProgramBase,new()
    {
        var func = step.Func;
        var args = step.Args ?? new List<object>();
        var method = typeof(T).GetMethod(func);

        if (method == null)
            throw new NotSupportedException($"Unsupported func: {func}");

        var methodParams = method.GetParameters();
        var paramValues = new object[methodParams.Length];
        
        for (int i = 0; i < args.Count; i++)
        {
            var param = methodParams[i];
            var arg = args[i];

            if (arg is Dictionary<string, object> dict)
            {
                // Handle reference or nested function
                if (dict.TryGetValue("@ref", out var refVal))
                {
                    paramValues[i] = prog.StepResults[(int)refVal];
                    continue;
                }

                if (dict.TryGetValue("@func", out var funcVal))
                {
                    var innerStep = dict.ToJson().FromJson<TypeChatStep>();
                    paramValues[i] = ProcessStep<T>(innerStep, prog);
                    continue;
                }
            }

            if (param.ParameterType == typeof(String) || param.ParameterType is { IsValueType: true, IsEnum: false })
            {
                // For value types, use Convert.ChangeType
                paramValues[i] = Convert.ChangeType(arg, Nullable.GetUnderlyingType(param.ParameterType) ?? param.ParameterType);
            }
            else
            {
                // For reference types, deserialize from JSON
                string jsonArg = arg.ToJson();
                paramValues[i] = JsonSerializer.DeserializeFromString(jsonArg, param.ParameterType);
            }
        }

        object result = null;
        if (typeof(Task).IsAssignableFrom(method.ReturnType))
        {
            // The method is async, await the result
            var task = (Task)method.Invoke(prog, paramValues);
            await task;

            // If the method returns a Task<T>, unwrap the result
            if (task.GetType().IsGenericType)
            {
                var resultProperty = task.GetType().GetProperty("Result");
                if (resultProperty != null)
                {
                    result = resultProperty.GetValue(task);
                }
            }
            // No else needed for Task, as result is already null
        }
        else
        {
            // For synchronous methods
            result = method.Invoke(prog, paramValues);
        }


        return result;
    }

}

public class SpotifyProgram : TypeChatProgramBase
{
    private SpotifyClient _spotifyClient;
    
    public override void Init()
    {
        _spotifyClient = new SpotifyClient(Config["SpotifyToken"]);
    }
    
    public async Task play(TrackList trackList, int? startIndex = null, int? count = null)
    {
        var tracks = trackList.Skip(startIndex ?? 0).Take(count ?? trackList.Count);
        var uris = tracks.Map(x => x.Uri);
        await _spotifyClient.Player.ResumePlayback(new PlayerResumePlaybackRequest {
            Uris = uris.ToList(),
        });
    }

    public async Task<TrackList> searchTracks(string query, SearchRequest.Types filterType = SearchRequest.Types.Track)
    {
        var result = await _spotifyClient.Search.Item(new SearchRequest(filterType, query));

        List<Track> tracks = new TrackList();
        var tracksList = new TrackList();
        switch (filterType)
        {
            case SearchRequest.Types.Album:
                var tRes = await _spotifyClient.Albums.GetSeveral(new AlbumsRequest(result.Albums.Items?.Select(x => x.Id).ToList() ?? new List<string>()));
                tracks.AddRange(tRes.Albums.Select(x =>
                    {
                        return x.Tracks.Items?.Select(y => new Track { Name = y.Name, Uri = y.Uri, Album = x.Name });
                    })
                    .SelectMany(x => x ?? new List<Track>())
                    .Select(x => new Track { Name = x.Name, Uri = x.Uri, Album = x.Name}));
                break;
            case SearchRequest.Types.Artist:
                var aRes = await _spotifyClient.Artists.GetSeveral(new ArtistsRequest(result.Artists.Items?.Select(x => x.Id).ToList() ?? new List<string>()));
                var aTracks = await _spotifyClient.Artists.GetTopTracks(aRes.Artists.First().Id, new ArtistsTopTracksRequest("US"));
                tracks.AddRange(aTracks.Tracks.Select(x => new Track { Name = x.Name, Uri = x.Uri, Album = x.Album.Name}));
                break;
            case SearchRequest.Types.Track:
                tracks = result.Tracks.Items?.Select(x => new Track { Name = x.Name, Uri = x.Uri }).ToList() ?? new TrackList();
                break;
        }
        
        tracksList.AddRange(tracks);

        return tracksList;
    }

    public async Task<List<Track>> getQueue()
    {
        // Logic to fetch and display upcoming tracks in the queue
        var queueResponse = await _spotifyClient.Player.GetQueue();
        var tracks = queueResponse.Queue.Where(x => x.Type == ItemType.Track).Select(x => x as SpotifyAPI.Web.FullTrack).ToList();
        return tracks.Select(x => new Track { Name = x.Name, Uri = x.Uri, Album = x.Album.Name}).ToList();
    }

    public async Task<CurrentlyPlayingContext> status()
    {
        var playback = await _spotifyClient.Player.GetCurrentPlayback();
        // Logic to display the current playback status
        return playback;
    }

    public async Task pause()
    {
        await _spotifyClient.Player.PausePlayback();
    }

    public async Task next()
    {
        await _spotifyClient.Player.SkipNext();
    }

    public async Task previous()
    {
        await _spotifyClient.Player.SkipPrevious();
    }

    public async Task shuffleOn()
    {
        await _spotifyClient.Player.SetShuffle(new PlayerShuffleRequest(true));
    }

    public async Task shuffleOff()
    {
        await _spotifyClient.Player.SetShuffle(new PlayerShuffleRequest(false));
    }

    public async Task resume()
    {
        await _spotifyClient.Player.ResumePlayback();
    }

    public async Task setVolume(int newVolumeLevel)
    {
        await _spotifyClient.Player.SetVolume(new PlayerVolumeRequest(newVolumeLevel));
    }
    
    public void unknownAction(string text)
    {
        // Logic for unknown actions
    }

    public void nonMusicQuestion(string text)
    {
        // Logic for non-music questions
    }

}

public class Track
{
    public string Name { get; set; }
    public string Uri { get; set; }
    public string Album { get; set; }
}

public class TrackList : List<Track> { }

public class Playlist : TrackList
{
    public string Id { get; set; }
}

public enum SpotifyFilterType
{
    Album,
    Artist,
    Track
}

public class TypeChatProgramBase
{
    public List<object> StepResults { get; set; } = new List<object>();
    public Dictionary<string, string> Config { get; set; }

    public TypeChatProgramBase()
    {
        Config = new Dictionary<string, string>();
    }

    public TypeChatProgramBase(Dictionary<string, string> config)
    {
        Config = config ?? new Dictionary<string, string>();
    }

    public virtual void Init()
    {
        
    }
}
