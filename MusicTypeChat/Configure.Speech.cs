using ServiceStack.AI;
using ServiceStack.IO;
using ServiceStack.GoogleCloud;
using Google.Cloud.Speech.V2;
using MusicTypeChat.ServiceInterface;
using MusicTypeChat.ServiceModel;

[assembly: HostingStartup(typeof(MusicTypeChat.ConfigureSpeech))]

namespace MusicTypeChat;

public class ConfigureSpeech : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            if (AppTasks.IsRunAsAppTask()) return;

            var speechProvider = context.Configuration.GetValue<string>("SpeechProvider");
            if (speechProvider == nameof(GoogleCloudSpeechToText))
            {
                AppHost.AssertGoogleCloudCredentials();
                services.AddSingleton<ISpeechToText>(c => new GoogleCloudSpeechToText(
                    X.Map(c.Resolve<AppConfig>(), config =>
                    {
                        var siteConfig = config.GetSiteConfig(Tags.Music);
                        return new GoogleCloudSpeechConfig
                        {
                            Project = config.Project,
                            Location = config.Location,
                            Bucket = siteConfig.Bucket,
                            RecognizerId = siteConfig.RecognizerId,
                            PhraseSetId = siteConfig.PhraseSetId,
                        };
                    })!, SpeechClient.Create()));
            }
            else if (speechProvider == nameof(WhisperApiSpeechToText))
            {
                services.AddSingleton<ISpeechToText, WhisperApiSpeechToText>();
            }
            else if (speechProvider == nameof(WhisperLocalSpeechToText))
            {
                services.AddSingleton<ISpeechToText>(c => new WhisperLocalSpeechToText {
                    WhisperPath = c.Resolve<AppConfig>().WhisperPath ?? ProcessUtils.FindExePath("whisper"),
                    TimeoutMs = c.Resolve<AppConfig>().NodeProcessTimeoutMs,
                });
            }
            else throw new NotSupportedException($"Unknown SpeechProvider '{speechProvider}'");
        })
        .ConfigureAppHost(appHost => {
            if (AppTasks.IsRunAsAppTask()) return;

            if (appHost.Resolve<ISpeechToText>() is IRequireVirtualFiles requireVirtualFiles)
            {
                requireVirtualFiles.VirtualFiles = appHost.VirtualFiles;
            }
        });
}