using ServiceStack;
using ServiceStack.OrmLite;
using MusicTypeChat.ServiceModel;

namespace MusicTypeChat.ServiceInterface;

public class PortalServices : Service
{
    public async Task<object> Any(AdminData request)
    {
        var tables = new (string Label, Type Type)[] 
        {
            ("Recordings",       typeof(Recording)),
            ("Chats",            typeof(Chat)),
        };
        var dialect = Db.GetDialectProvider();
        var totalSql = tables.Map(x => $"SELECT '{x.Label}', COUNT(*) FROM {dialect.GetQuotedTableName(x.Type.GetModelMetadata())}")
            .Join(" UNION ");
        var results = await Db.DictionaryAsync<string,int>(totalSql);
        
        return new AdminDataResponse {
            PageStats = tables.Map(x => new PageStats {
                Label = x.Label, 
                Total = results[x.Label],
            })
        };
    }
}
