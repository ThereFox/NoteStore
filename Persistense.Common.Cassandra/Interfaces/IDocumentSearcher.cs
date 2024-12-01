using Persistense.Common.Cassandra.DTOs;

namespace Persistense.Common.Cassandra.Interfaces;

public interface IDocumentSearcher
{
    public Task<IList<MatchInfo>> GetDocumentWithRelatedText(string text);
}