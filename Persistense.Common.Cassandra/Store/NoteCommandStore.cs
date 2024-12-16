using Application.Interfaces.Stores;
using Cassandra.Mapping;
using CSharpFunctionalExtensions;
using NoteStore.Domain;
using Persistense.Common.Cassandra.Interfaces;

namespace Persistense.Common.Cassandra.Store;

public class NoteCommandStore : INoteCommandStore
{
    private readonly ICqlWriteAsyncClient _writer;
    private readonly ICqlQueryAsyncClient _query;
    private readonly IDocumentStore _documentStore;

    public NoteCommandStore(ICqlQueryAsyncClient client, ICqlWriteAsyncClient writer, IDocumentStore documentStore)
    {
        _query = client;
        _writer = writer;
        _documentStore = documentStore;
    }
    
    public async Task<Result> AddNote(Note note)
    {
        try
        {
            var saveFileResult = await _documentStore.SaveContent(note);

            if (saveFileResult.IsFailure)
            {
                return saveFileResult;
            }

            var fileId = saveFileResult.Value;
            
            var cql = new Cql(
                @$"

                INSERT INTO noteStore.notes (Id, Version, PartitionId, ContentId, CreatorId, CreatorName, Header)
                VALUES (?, ?, ?, ?, ?, ?, ?);
                ", note.Id, 1, note.Group.Value, fileId, note.Owner.Id, note.Owner.Name, note.Content.Title
            );
            
            await _writer.ExecuteAsync(cql);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result> UpdateNote(Note note)
    {
        try
        {
            var saveFileResult = await _documentStore.SaveContent(note);

            if (saveFileResult.IsFailure)
            {
                return saveFileResult;
            }

            var fileId = saveFileResult.Value;

            var oldVersionCQL = new Cql(
                $@"
                    SELECT Version
                    FROM noteStore.notes
                    WHERE Id = ?
                    ORDER BY Version DESC
                    LIMIT 1;
                "
            );

            var lastVersionFromDB = await _query.SingleOrDefaultAsync<int>(oldVersionCQL);
            
            var newVersion = lastVersionFromDB != null ? lastVersionFromDB + 1 : 1;
            
            var createNewCql = new Cql(
                @$"

                INSERT INTO noteStore.notes (Id, Version, PartitionId, ContentId, CreatorId, CreatorName, Header)
                VALUES (?, ?, ?, ?, ?, ?);
                ", note.Id, newVersion, note.Group.Value, fileId, note.Owner.Id, note.Owner.Name, note.Content.Title
            );
            
            await _writer.ExecuteAsync(createNewCql);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result> DeleteNote(Guid noteId)
    {
        try
        {
            var cql = new Cql(
                $@"
                    UPDATE noteStore.notes
                    SET PartitionId = -1
                    WHERE Id = ?;
                ", noteId
            );

            await _writer.UpdateAsync(cql);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }
}