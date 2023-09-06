using ServiceStack;
using SpotifyAPI.Web;

namespace MusicTypeChat.ServiceInterface;

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