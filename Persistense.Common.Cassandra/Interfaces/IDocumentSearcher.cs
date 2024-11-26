namespace Persistense.Common.Cassandra.Interfaces;

public interface IDocumentSearcher
{
    public IList<Guid> GetDocumentWithRelatedText(string text);
}