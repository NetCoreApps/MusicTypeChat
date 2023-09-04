using System.Runtime.Serialization;
using ServiceStack;

namespace MusicTypeChat.ServiceModel;

public class GptRequestBase<T> : IGptRequest<T>
{
    public string UserRequest { get; set; }
    public Dictionary<string,object>? PromptContext { get; set; }
}

public interface IGptRequest<T> : IReturn<T>
{
    string UserRequest { get; set; }
    Dictionary<string,object>? PromptContext { get; set; }
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
    public List<object>? Args { get; set; }
}

[DataContract]
public class TypeChatRefArg
{
    [DataMember(Name = "@ref")]
    public int Ref { get; set; }
}