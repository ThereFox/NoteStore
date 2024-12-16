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
            var cql = new Cql(
                @"
                SELECT Id, PartitionId, ContentId, CreatorId, CreatorName, Header
                FROM notestore.notes
                WHERE PartitionId = ?
                ORDER BY Id ASC
                LIMIT ?
                ", group.Value, count);
            
            
            var notesFetch = await _queryClient.FetchAsync<NoteDTO>(
                cql
            );

            var notes = notesFetch.ToList();
            
            if (notes is null || notes.Any() == false)
            {
                return [];
            }
            
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

            if (relatedElements is null || relatedElements.Any() == false)
            {
                return [];
            }
            
            var ElementFromDB = (await _queryClient.FetchAsync<NoteDTO>(
                @"                
                SELECT Id, PartitionId, ContentId, CreatorId, CreatorName, Header
                FROM notestore.notes
                WHERE PartitionId > -1 AND Id IN ?
                ALLOW FILTERING
                ",
                relatedElements.Select(ex => ex.ElementId)
                )).ToList();

            if (ElementFromDB.Any() == false)
            {
                return [];
            }
            
            var result = new System.Collections.Generic.List<NoteMatch>();
            
            foreach (var element in ElementFromDB)
            {
                var contentById = relatedElements.FirstOrDefault(ex => ex.ElementId == element.Id);
                
                if (contentById == null)
                {
                    continue;
                }

                var validateInfo = element.ToNoteShortInfo();

                if (validateInfo.IsFailure)
                {
                    continue;
                }
                
                var match = new NoteMatch(validateInfo.Value, contentById.MatchedText);
                
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
                    SELECT Id, PartitionId, ContentId, CreatorId, CreatorName, Header
                    FROM notestore.notes
                    WHERE Id = ?
                    LIMIT 1
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