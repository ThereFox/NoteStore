using NoteStore.Domain;

namespace Application.DTOs;

public class NoteMatch
{
    public NoteShortInfo Note { get; private set; }
    public string MatchString { get; private set; }

    public NoteMatch(NoteShortInfo note, string matchString)
    {
        Note = note;
        MatchString = matchString;
    }
}