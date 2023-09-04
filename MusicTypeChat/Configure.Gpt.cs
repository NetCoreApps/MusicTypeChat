using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Reliability;
using MusicTypeChat.ServiceInterface;
using MusicTypeChat.ServiceModel;
using ServiceStack.Gpt;

[assembly: HostingStartup(typeof(MusicTypeChat.ConfigureGpt))]

namespace MusicTypeChat;

public class ConfigureGpt : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton<IPromptProvider>(c =>
                new MusicChatPromptProvider(c.Resolve<AppConfig>()));
            
            // Call Open AI Chat API directly without going through node TypeChat
            var gptProvider = context.Configuration.GetValue<string>("TypeChatProvider");
            if (gptProvider == nameof(KernelTypeChatProvider))
            {
                var kernel = Kernel.Builder
                    .WithOpenAIChatCompletionService(
                        Environment.GetEnvironmentVariable("OPENAI_MODEL")!, 
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
                    .Build();
                services.AddSingleton(kernel);
                services.AddSingleton<ITypeChatProvider>(c =>  new KernelTypeChatProvider(c.Resolve<IKernel>()));
            }
            else if (gptProvider == nameof(NodeTypeChatProvider))
            {
                // Call Open AI Chat API through node TypeChat
                services.AddSingleton<ITypeChatProvider>(c => new NodeTypeChatProvider());
            }
            else throw new NotSupportedException($"Unknown TypeChatProvider: {gptProvider}");
        });
}