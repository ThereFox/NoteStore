using Application.Interfaces.Stores;
using Cassandra;
using Cassandra.Mapping;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;
using Persistense.Common.Cassandra.DTOs;
using Persistense.Common.Cassandra.Interfaces;

namespace Persistense.Common.Cassandra.Store;

public class NoteStore : INoteStore
{
    private readonly IDocumentSearcher _searcher;
    private readonly IDocumentStore _store;

    private readonly ICqlQueryAsyncClient _queryClient;
    
    public NoteStore(IDocumentSearcher searcher, IDocumentStore store)
    {
        _searcher = searcher;
        _store = store;
    }
    
    public async Task<IList<Note>> GetAllByGroup(NoteGroup group)
    {
        try
        {
            var notes = await _queryClient.FetchAsync<NoteDTO>(
                @"
                SELECT Id, PartitionId, ContentId, CreatorId
                FROM notes
                WHERE group_id = @group
                ORDER BY CreationDate DESC
                LIMIT 15
                ", new { group = group.Value }
            );

            return notes.ToList();
        }
        catch (Exception ex)
        {
            re
        }
    }

    public async Task<IList<Note>> GetAllWithRelatedText(string relatedText)
    {
        try
        {
            var relatedElements = await _searcher.GetDocumentWithRelatedText(relatedText);

            var ElementFromDB = await _queryClient.FetchAsync<NoteDTO>(
                @"                
                SELECT Id, PartitionId, ContentId, CreatorId
                FROM notes
                WHERE Id IN @relatedElements
                ORDER BY CreationDate DESC
                ",
                new { relatedElements = relatedElements }
                );

            return ElementFromDB.ToList();
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Note GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}