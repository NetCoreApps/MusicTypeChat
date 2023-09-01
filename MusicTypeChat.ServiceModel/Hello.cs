using System.Runtime.Serialization;
using ServiceStack;

namespace MusicTypeChat.ServiceModel;

[Route("/hello")]
[Route("/hello/{Name}")]
public class Hello : IReturn<HelloResponse>
{
    public string? Name { get; set; }
}

public class ProcessSpotifyCommand : GptRequestBase<SpotifyCommand>
{
}

public class HelloResponse
{
    public string Result { get; set; } = default!;
}

[DataContract]
public class SpotifyCommand : TypeChatProgramResponse
{
    
}