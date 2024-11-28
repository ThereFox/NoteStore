using CSharpFunctionalExtensions;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;

namespace Persistense.Common.Cassandra.Interfaces;

public interface IDocumentStore
{
    public Task<Result<NoteContent>> GetContentByKey(Guid key);
    public Task<Result<Guid>> SaveContent(Note note);
}