using CSharpFunctionalExtensions;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;

namespace Persistense.Serching.Elastick.Document;

public record NoteContentDocument
(
    Guid Id,
    string OwnerName,
    string Title,
    string Description,
    string Content
);

public static class NoteContentDocumentConverter
{
    public static NoteContentDocument ToDocument(this Note note)
    {
        return new NoteContentDocument(
            note.Id,
            note.Owner.Name,
            note.Content.Title,
            note.Content.Descriplion,
            note.Content.Content
        );
    }

    public static Result<NoteContent> ToContent(this NoteContentDocument document, DateTime updatedAt)
    {
        var content = NoteContent.Create(updatedAt, document.Title, document.Description, document.Content);

        return content;
    }
    
}