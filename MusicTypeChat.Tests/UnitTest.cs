using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using MusicTypeChat.ServiceInterface;

namespace MusicTypeChat.Tests;

public class UnitTest
{
    private readonly ServiceStackHost appHost;

    public UnitTest()
    {
        appHost = new BasicAppHost().Init();
        appHost.Container.AddTransient<GptServices>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => appHost.Dispose();
}