using Application.DTOs;
using Application.Interfaces.Stores;
using Cassandra;
using Cassandra.Mapping;
using CSharpFunctionalExtensions;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;
using Persistense.Common.Cassandra.DTOs;
using Persistense.Common.Cassandra.Interfaces;

namespace Persistense.Common.Cassandra.Store;

public class NoteQueryStore : INoteQueryStore
{
    private readonly IDocumentSearcher _searcher;
    private readonly IDocumentStore _store;

    private readonly ICqlQueryAsyncClient _queryClient;
    
    public NoteQueryStore(ICqlQueryAsyncClient client, IDocumentSearcher searcher, IDocumentStore store)
    {
        _queryClient = client;
        _searcher = searcher;
        _store = store;
    }
    
    public async Task<IList<NoteShortInfo>> GetNByGroup(NoteGroup group, int count)
    {
        try
        {
            var notes = await _queryClient.FetchAsync<NoteDTO>(
                @"
                SELECT Id, PartitionId, ContentId, CreatorId
                FROM notestore.notes
                WHERE group_id = ?
                ORDER BY CreationDate DESC
                LIMIT ?
                ", group.Value,count
            );

            return notes.ToNoteStortInfoList();
        }
        catch (Exception ex)
        {
            return [];
        }
    }

    public async Task<IList<NoteMatch>> GetAllWithRelatedText(string relatedText)
    {
        try
        {
            var relatedElements = await _searcher.GetDocumentWithRelatedText(relatedText);

            var ElementFromDB = await _queryClient.FetchAsync<NoteDTO>(
                @"                
                SELECT Id, PartitionId, ContentId, CreatorId
                FROM notestore.notes
                WHERE Id IN ?
                ",
                relatedElements
                );

            var result = new System.Collections.Generic.List<NoteMatch>();
            
            foreach (var element in relatedElements)
            {
                var noteById = ElementFromDB.First(ex => ex.ContentId == element.ElementId);

                if (noteById == null)
                {
                    continue;
                }

                var validateInfo = noteById.ToNoteShortInfo();

                if (validateInfo.IsFailure)
                {
                    continue;
                }
                
                var match = new NoteMatch(validateInfo.Value, element.MatchedText);
                
                result.Add(match);
            }
            
            return result;
            
        }
        catch (Exception ex)
        {
            return [];
        }
    }

    public async Task<Result<Note>> GetById(Guid id)
    {
        try
        {
            var getElementByIdCQL = new Cql(
                @"
                    SELECT Id, PartitionId, ContentId, CreatorId
                    FROM notestore.notes
                    WHERE Id = ?
                ", id);

            var noteById = (await _queryClient.FetchAsync<NoteDTO>(getElementByIdCQL)).FirstOrDefault();

            if (noteById == null)
            {
                return Result.Failure<Note>("Note not found");
            }
            
            noteById.CreatorName ??= "Unknown";
            noteById.Header ??= "Empty";
            
            var noteContentById = await _store.GetContentByKey(noteById.ContentId);

            if (noteContentById.IsFailure)
            {
                return Result.Failure<Note>("Note content not found");
            }
            
            var content = noteContentById.Value;

            return noteById.FormatNote(content);

        }
        catch (Exception ex)
        {
            return Result.Failure<Note>(ex.Message);
        }
    }
}