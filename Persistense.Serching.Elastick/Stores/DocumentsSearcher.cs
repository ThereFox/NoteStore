using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Persistense.Common.Cassandra.DTOs;
using Persistense.Common.Cassandra.Interfaces;
using Persistense.Serching.Elastick.Document;
using Persistense.Serching.Elastick.Extensions;

namespace Persistense.Serching.Elastick.Stores;

public class DocumentsSearcher : IDocumentSearcher
{
    private readonly ElasticsearchClient _client;
    private readonly string _indexName;

    public DocumentsSearcher(ElasticsearchClient client, string indexName)
    {
        _client = client;
        _indexName = indexName;
    }
    
    public async Task<IList<MatchInfo>> GetDocumentWithRelatedText(string text)
    {


        var searchResult = await _client.SearchAsync<NoteContentDocument>(
            ex => ex.Index(_indexName)
                .From(0)
                .Size(15)
                .Query(query => query.Term(term => term.Field(field => field.Content).Value(text)))
        );

        if (searchResult.Hits.Count == 0)
        {
            return new List<MatchInfo>();
        }
        
        return searchResult.AsMatchInfo();
        
    }
}