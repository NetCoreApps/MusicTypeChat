using MusicTypeChat.ServiceModel;
using ServiceStack;
using ServiceStack.Gpt;
using ServiceStack.Text;

namespace MusicTypeChat.ServiceInterface;

public class MusicService : Service
{
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