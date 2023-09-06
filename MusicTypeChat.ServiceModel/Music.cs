using System.Runtime.Serialization;
using MusicTypeChat.ServiceModel.Types;
using ServiceStack;

namespace MusicTypeChat.ServiceModel;

public class ProcessSpotifyCommand : IReturn<SpotifyCommand>
{
    public string UserMessage { get; set; }
}

public class HelloResponse
{
    public string Result { get; set; } = default!;
}

[DataContract]
public class SpotifyCommand : TypeChatProgramDetails
{
    
}


[DataContract]
public class TypeChatProgramResponse
{
    [DataMember(Name = "@steps")]
    public List<TypeChatStep> Steps { get; set; }
}

[DataContract]
public class TypeChatStep
{
    [DataMember(Name = "@func")]
    public string Func { get; set; }
    [DataMember(Name = "@args")]
    public List<object> Args { get; set; }
}

[DataContract]
public class TypeChatRefArg
{
    [DataMember(Name = "@ref")]
    public int Ref { get; set; }
}

[AutoPopulate(nameof(Recording.CreatedDate),  Eval = "utcNow")]
[AutoPopulate(nameof(Recording.IpAddress),  Eval = "Request.RemoteIp")]
public class TranscribeAudio : ICreateDb<Recording>, IReturn<Recording>
{
    [Input(Type="file"), UploadTo("recordings")]
    public string Path { get; set; }
}

public class TypeChatProgramDetails
{
    public List<object> StepResults { get; set; } = new List<object>();
    public object? Result { get; set; }
    public List<TypeChatStep> Steps { get; set; } = new List<TypeChatStep>();
}