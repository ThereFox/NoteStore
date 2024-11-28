using CSharpFunctionalExtensions;

namespace NoteStore.Domain.ValueObjects;

public class NoteContent : ValueObject
{
    public DateTime VersionCreateTime { get; private set; }
    
    public string Title { get; private set; }
    public string Descriplion { get; private set; }
    public string Content { get; private set; }

    public static Result<NoteContent> Create(DateTime versionCreateTime, string title, string descriplion, string content)
    {
        if (versionCreateTime >= DateTime.Now)
        {
            return Result.Failure<NoteContent>("Version created earlier than today");
        }
        
        return Result.Success(new NoteContent(versionCreateTime, title, descriplion, content));
    }

    protected NoteContent(DateTime versionCreateTime, string title, string descriplion, string content)
    {
        VersionCreateTime = versionCreateTime;
        Title = title;
        Descriplion = descriplion;
        Content = content;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Title;
        yield return Descriplion;
        yield return Content;
    }
}