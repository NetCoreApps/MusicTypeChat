using ServiceStack;

namespace MusicTypeChat.ServiceModel;

public class AdminData : IGet, IReturn<AdminDataResponse> {}
public class PageStats
{
    public string Label { get; set; }
    public int Total { get; set; }
}
public class AdminDataResponse
{
    public List<PageStats> PageStats { get; set; }
}