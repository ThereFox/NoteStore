using Elastic.Clients.Elasticsearch;
using Persistense.Common.Cassandra.DTOs;

namespace Persistense.Serching.Elastick.Extensions;

public static class AsMatchInfoExtension
{
    public static IList<MatchInfo> AsMatchInfo<T>(this SearchResponse<T> result)
    {
        var list = new List<MatchInfo>();
        foreach (var hits in result.Hits)
        {
            var hit = new MatchInfo(Guid.Parse(hits.Id), hits.Highlight.Values.First().First());
            list.Add(hit);
        }
        
        return list;
    }
}