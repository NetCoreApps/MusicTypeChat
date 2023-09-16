using MusicTypeChat.ServiceInterface;
using MusicTypeChat.ServiceModel;
using Microsoft.SemanticKernel;
using ServiceStack.AI;

[assembly: HostingStartup(typeof(MusicTypeChat.ConfigureGpt))]

namespace MusicTypeChat;

public class ConfigureGpt : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton<MusicPromptProvider>();
            services.AddSingleton<IPromptProviderFactory>(c => new PromptProviderFactory {
                Providers = {
                    [Tags.Music] = c.Resolve<MusicPromptProvider>()
                }
            });
            
            // Call Open AI Chat API directly without going through node TypeChat
            var gptProvider = context.Configuration.GetValue<string>("TypeChatProvider");
            if (gptProvider == nameof(KernelTypeChat))
            {
                var kernel = Kernel.Builder.WithOpenAIChatCompletionService(
                        Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-3.5-turbo", 
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
                    .Build();
                services.AddSingleton(kernel);
                services.AddSingleton<ITypeChat>(c => new KernelTypeChat(c.Resolve<IKernel>()));
            }
            else if (gptProvider == nameof(NodeTypeChat))
            {
                // Call Open AI Chat API through node TypeChat
                services.AddSingleton<ITypeChat>(c => new NodeTypeChat());
            }
            else throw new NotSupportedException($"Unknown TypeChat Provider: {gptProvider}");
        });
}