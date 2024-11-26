using CSharpFunctionalExtensions;

namespace NoteStore.Domain.ValueObjects;

public class NoteContent : ValueObject
{
    public DateTime VersionCreateTime { get; private set; }
    
    public string Title { get; private set; }
    public string Descriplion { get; private set; }
    public string Content { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}