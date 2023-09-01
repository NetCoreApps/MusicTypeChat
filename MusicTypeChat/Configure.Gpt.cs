using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Reliability;
using MusicTypeChat.ServiceInterface;
using MusicTypeChat.ServiceModel;

[assembly: HostingStartup(typeof(MusicTypeChat.ConfigureGpt))]

namespace MusicTypeChat;

public class ConfigureGpt : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            // Call Open AI Chat API directly without going through node TypeChat
            var gptProvider = context.Configuration.GetValue<string>("GptChatProvider");
            if (gptProvider == nameof(KernelChatProvider<SpotifyCommand>))
            {
                var kernel = Kernel.Builder
                    .WithOpenAIChatCompletionService(
                        Environment.GetEnvironmentVariable("OPENAI_MODEL")!, 
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
                    .WithRetryHandlerFactory(new DefaultHttpRetryHandlerFactory(
                        new HttpRetryConfig
                        {
                            MaxRetryCount = 3,
                        }))
                    .Build();
                services.AddSingleton(kernel);
                services.AddSingleton<ITypeChatProvider<SpotifyCommand>>(c => 
                    new KernelChatProvider<SpotifyCommand>(c.Resolve<AppConfig>(), c.Resolve<IKernel>()));
            }
            else if (gptProvider == nameof(NodeTypeChatProvider<SpotifyCommand>))
            {
                // Call Open AI Chat API through node TypeChat
                services.AddSingleton<ITypeChatProvider<SpotifyCommand>>(c =>
                    new NodeTypeChatProvider<SpotifyCommand>(c.Resolve<AppConfig>()));
            }
        });
}