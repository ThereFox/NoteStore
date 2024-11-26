using CSharpFunctionalExtensions;
using NoteStore.Domain.ValueObjects;

namespace NoteStore.Domain;

public class Note : Entity<Guid>
{
    public NoteOwner Owner { get; private set; }
    
    public NoteGroup Group { get; private set; }
    
    public NoteContent Content { get; private set; }
    
    
    private Note(Guid id, NoteOwner owner, NoteGroup group, NoteContent content)
    {
        Id = id;
        Owner = owner;
        Group = group;
        Content = content;
    }
}