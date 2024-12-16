using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
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
                .Highlight(subEx => subEx
                    .HighlightQuery( query =>
                        query.Match(
                                term => term.Field(
                                        field => field.Content
                                    )
                                    .Query(text)))
                    
                    .Fields(ex => ex.Add(
                        new Field("content"), 
                        descriptor => 
                            descriptor
                                .Type(HighlighterType.Plain)
                                .Fragmenter(HighlighterFragmenter.Span)
                                .PreTags(["<span>"])
                                .PostTags(["</span>"])
                                .FragmentSize(150)
                                .NoMatchSize(150)
                                .NumberOfFragments(1)
                        ))
                    )
                .Query(
                    query => 
                        query.Match(
                            term => term.Field(
                                field => field.Content
                                )
                                .Query(text)))
        );

        if (searchResult.Hits.Count == 0)
        {
            return new List<MatchInfo>();
        }
        
        return searchResult.AsMatchInfo();
        
    }
}