using CSharpFunctionalExtensions;

namespace NoteStore.Domain.ValueObjects;

public class NoteGroup : ValueObject
{
    public static NoteGroup Common => new NoteGroup(0);
    public static NoteGroup Tips => new NoteGroup(1);
    public static NoteGroup Work => new NoteGroup(2);
    public static NoteGroup OwnLife => new NoteGroup(3);
    
    private static List<NoteGroup> _all = [Common, Tips, Work, OwnLife];
    
    public int Value { get; private set; }

    protected NoteGroup(int value)
    {
        Value = value;
    }

    public static Result<NoteGroup> Create(int value)
    {
        if (_all.Any(ex => ex.Value == value) == false)
        {
            return Result.Failure<NoteGroup>("unsupported value");
        }

        return Result.Success(new NoteGroup(value));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}