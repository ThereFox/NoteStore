using NoteStore.Domain.ValueObjects;

namespace Persistense.Common.Cassandra.Interfaces;

public interface IDocumentGetter
{
    public NoteContent GetContentByKey(string key);
}