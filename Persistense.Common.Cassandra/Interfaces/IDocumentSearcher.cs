namespace Persistense.Common.Cassandra.Interfaces;

public interface IDocumentSearcher
{
    public Task<IList<Guid>> GetDocumentWithRelatedText(string text);
}