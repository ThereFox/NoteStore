using CSharpFunctionalExtensions;

namespace NoteStore.Domain;

public class NoteOwner : Entity<Guid>
{
    public string Name { get; private set; }

    private NoteOwner(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Result<NoteOwner> Create(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<NoteOwner>($"{nameof(name)} cannot be empty");
        }
        
        return Result.Success(new NoteOwner(id, name));
    }
}