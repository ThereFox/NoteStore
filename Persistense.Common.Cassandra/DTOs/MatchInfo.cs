namespace Persistense.Common.Cassandra.DTOs;

public class MatchInfo
{
    public Guid ElementId { get; private set; }
    public string MatchedText { get; private set; }

    public MatchInfo(Guid elementId, string matchedText)
    {
        ElementId = elementId;
        MatchedText = matchedText;
    }
}