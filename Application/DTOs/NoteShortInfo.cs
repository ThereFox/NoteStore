using NoteStore.Domain.ValueObjects;

namespace Application.DTOs;

public class NoteShortInfo
{
    public Guid Id { get; private set; }
    public NoteGroup Group { get; private set; }
    public string CreatorName { get; private set; }
    public string Header { get; private set; }

    public NoteShortInfo(Guid id, NoteGroup group, string creatorName, string header)
    {
        Id = id;
        Group = group;
        CreatorName = creatorName;
        Header = header;
    }
}