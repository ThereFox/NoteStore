using CSharpFunctionalExtensions;

namespace NoteStore.Domain;

public class NoteOwner : Entity<Guid>
{
    public string Name { get; private set; }
}