using CSharpFunctionalExtensions;
using Elastic.Clients.Elasticsearch;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;
using Persistense.Common.Cassandra.Interfaces;
using Persistense.Serching.Elastick.Document;
using Result = CSharpFunctionalExtensions.Result;

namespace Persistense.Serching.Elastick.Stores;

public class DocumentStore : IDocumentStore
{
    private readonly ElasticsearchClient _client;
    
    private readonly string _indexName;

    public DocumentStore(ElasticsearchClient client, string indexName)
    {
        _client = client;
        _indexName = indexName;
    }
    
    public async Task<Result<NoteContent>> GetContentByKey(Guid key)
    {
        var document = await _client
            .GetAsync<NoteContentDocument>(
                new Id(key.ToString()),
                ex => 
                    ex.Index(_indexName)
                        .Id(key.ToString())
            );

        if (document.IsValidResponse == false || document.ElasticsearchServerError != null || document.Source == null)
        {
            return Result.Failure<NoteContent>("Could not get content by key");
        }

        return document.Source.ToContent(DateTime.Today);

    }

    public async Task<Result<Guid>> SaveContent(Note note)
    {
        try
        {
            var document = note.ToDocument();

            var indexResult = await _client.IndexAsync(document,
                ex =>
                    ex
                        .Index(_indexName)
                        .Id(document.Id)    
                );

            if (indexResult.IsValidResponse == false ||
                indexResult.Result != Elastic.Clients.Elasticsearch.Result.Created)
            {
                return Result.Failure<Guid>("invalid result state");
            }


            return Result.Success(document.Id);
        }
        catch (Exception e)
        {
            return Result.Failure<Guid>(e.Message);
        }
    }
}