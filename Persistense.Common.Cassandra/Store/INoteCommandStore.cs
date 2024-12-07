using Application.Interfaces.Stores;
using Cassandra.Mapping;
using CSharpFunctionalExtensions;
using NoteStore.Domain;
using Persistense.Common.Cassandra.Interfaces;

namespace Persistense.Common.Cassandra.Store;

public class NoteCommandStore : INoteCommandStore
{
    private readonly ICqlWriteAsyncClient _writer;
    private readonly IDocumentStore _documentStore;

    public NoteCommandStore(ICqlWriteAsyncClient writer, IDocumentStore documentStore)
    {
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

                INSERT INTO noteStore.notes (Id, PartitionId, ContentId, CreatorId, CreatorName, Header)
                VALUES (?, ?, ?, Uuid(), 'Test', 'Test');
                ", note.Id, note.Group.Value, fileId
            );
            
            await _writer.ExecuteAsync(cql);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    public Task<Result> UpdateNote(Note note)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteNote(Guid noteId)
    {
        throw new NotImplementedException();
    }
}