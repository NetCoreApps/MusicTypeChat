using ServiceStack;
using MusicTypeChat.ServiceModel;
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
    
    public ITypeChatProvider<SpotifyCommand> TypeChatProvider { get; set; }
    
 
    public async Task<object> Post(ProcessSpotifyCommand request)
    {
        var program = await TypeChatProvider.ProcessAsync(request);
        var session = GetSession().GetAuthTokens("spotify");
        if(session == null)
            throw new Exception("Spotify session not found");
        var programResult = await BindAndRun<SpotifyProgram>(program, new Dictionary<string, string>
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
        var args = step.Args;
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

            if (param.ParameterType == typeof(String) || param.ParameterType.IsValueType)
            {
                // For value types, use Convert.ChangeType
                paramValues[i] = Convert.ChangeType(arg, param.ParameterType);
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
    private TrackList _lastShownTrackList = new TrackList();
    
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

    public async Task<TrackList> searchTracks(string query)
    {
        var result = await _spotifyClient.Search.Item(new SearchRequest(SearchRequest.Types.Track, query));
        
        var tracks =  result.Tracks.Items?.Select(x => new Track { Name = x.Name, Uri = x.Uri }).ToList();
        var tracksList = new TrackList();
        if (tracks != null)
        {
            tracksList.AddRange(tracks);
        }

        return tracksList;
    }
    
    public void printTracks(TrackList trackList)
    {
        // Logic to print the tracks to console or some other medium
    }

    public async Task getQueue()
    {
        // Logic to fetch and display upcoming tracks in the queue
    }

    public async Task status()
    {
        var playback = await _spotifyClient.Player.GetCurrentPlayback();
        // Logic to display the current playback status
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

    public async Task listDevices()
    {
        var devices = await _spotifyClient.Player.GetAvailableDevices();
        // Logic to list available devices
    }

    public async Task selectDevice(string keyword)
    {
        var devices = await _spotifyClient.Player.GetAvailableDevices();
        // Logic to select a device based on the keyword
    }

    public async Task setVolume(int newVolumeLevel)
    {
        await _spotifyClient.Player.SetVolume(new PlayerVolumeRequest(newVolumeLevel));
    }

    public async Task changeVolume(int volumeChangeAmount)
    {
        var playback = await _spotifyClient.Player.GetCurrentPlayback();
        int? newVolume = playback.Device.VolumePercent + volumeChangeAmount;
        await _spotifyClient.Player.SetVolume(new PlayerVolumeRequest(newVolume ?? 33));
    }
    
    public TrackList getLastTrackList()
    {
        return _lastShownTrackList;
    }

    public async Task listPlaylists()
    {
        var playlists = await _spotifyClient.Playlists.CurrentUsers();
        // Logic to list all playlists
    }

    public async Task<Playlist> getPlaylist(string name)
    {
        var playlists = await _spotifyClient.Playlists.CurrentUsers();
        // Logic to get playlist by name
        // Get Playlist by name
        var playlist = playlists.Items?.FirstOrDefault(x => x.Name == name);
        if (playlist == null)
        {
            throw new Exception("Playlist not found");
        }
        // Get tracks from playlist
        return new Playlist()
        {
            Id = playlist.Id
        };
    }

    public async Task<TrackList> getAlbum(string name)
    {
        var album = await _spotifyClient.Albums.Get(name);
        // Logic to get album by name
        // Get tracks from album
        var tracks = album.Tracks.Items?.Select(x => new Track { Name = x.Name, Uri = x.Uri }).ToList();
        var tracksList = new TrackList();
        if (tracks != null) tracksList.AddRange(tracks);
        return tracksList;
    }

    public async Task<TrackList> getFavorites(int? count = null)
    {
        var favoriteTracks = await _spotifyClient.Library.GetTracks();
        // Logic to get favorite tracks
        var tracks = favoriteTracks.Items?.Select(x => new Track { Name = x.Track.Name, Uri = x.Track.Uri }).ToList();
        var tracksList = new TrackList();
        if (tracks != null) tracksList.AddRange(tracks);

        return tracksList;
    }

    public TrackList filterTracks(TrackList trackList, string filterType, string filter, bool? negate = false)
    {
        // Logic to filter tracks
        throw new NotImplementedException();
    }

    public async Task createPlaylist(TrackList trackList, string name)
    {
        var createRequest = new PlaylistCreateRequest(name);
        // get the authorized user id.
        var user = await _spotifyClient.UserProfile.Current();
        var playlist = await _spotifyClient.Playlists.Create(user.Id, createRequest);
        // Logic to add tracks to the playlist
    }

    public async Task deletePlaylist(Playlist playlist)
    {
        // Logic to delete a playlist
        // Not supported by the API
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
}

public class TrackList : List<Track> { }

public class Playlist : TrackList
{
    public string Id { get; set; }
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
