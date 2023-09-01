using Funq;
using MusicTypeChat.ServiceInterface;
using ServiceStack.DataAnnotations;
using ServiceStack.NativeTypes;
using SpotifyAPI.Web;

[assembly: HostingStartup(typeof(MusicTypeChat.AppHost))]

namespace MusicTypeChat;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context,services) => {
            // Configure ASP.NET Core IOC Dependencies
            var appConfig = new AppConfig();
            context.Configuration.Bind(nameof(AppConfig), appConfig);
            services.AddSingleton(appConfig);

            if (!AppTasks.IsRunAsAppTask())
            {
                appConfig.NodePath = ProcessUtils.FindExePath("node")
                                     ?? throw new Exception("Could not resolve path to node");
                appConfig.FfmpegPath = ProcessUtils.FindExePath("ffmpeg");
                appConfig.WhisperPath = ProcessUtils.FindExePath("whisper");
            }
        });

    public AppHost() : base("MusicTypeChat", typeof(MyServices).Assembly) {}

    public override void Configure(Container container)
    {
        SetConfig(new HostConfig {
        });
        
        Plugins.Add(new CorsFeature(new[] {
            "http://localhost:5173", //vite dev
        }, allowCredentials:true));
    }
}
