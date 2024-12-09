using Application.DTOs;
using CSharpFunctionalExtensions;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;

namespace Persistense.Common.Cassandra.DTOs;

public static class NoteConverterExtension
{
    public static Result<NoteShortInfo> ToNoteShortInfo(this NoteDTO note)
    {
        var validateNoteGroup = NoteGroup.Create(note.PartitionId);

        if (validateNoteGroup.IsFailure)
        {
            return validateNoteGroup.ConvertFailure<NoteShortInfo>();
        }
        
        return new NoteShortInfo(note.Id, validateNoteGroup.Value, note.CreatorName, note.Header);
    }

    public static IList<NoteShortInfo> ToNoteStortInfoList(this IList<NoteDTO> notes)
    {
        if (notes is null || notes.Any() == false)
        {
            return [];
        }
        
        return notes
            .Select(ex =>
                {
                    var validateShortInfo = ex.ToNoteShortInfo();
                    return validateShortInfo.IsSuccess ? validateShortInfo.Value : null;
                })
            .Where(ex => ex is not null)
            .ToList();
    }

    public static Result<Note> FormatNote(this NoteDTO note, NoteContent content)
    {
        var validateOwner = NoteOwner.Create(note.CreatorId, note.CreatorName);

        if (validateOwner.IsFailure)
        {
            return validateOwner.ConvertFailure<Note>();
        }
        
        var validateNoteGroup = NoteGroup.Create(note.PartitionId);

        if (validateNoteGroup.IsFailure)
        {
            return validateNoteGroup.ConvertFailure<Note>();
        }
        
        return Note.Create(note.Id, validateOwner.Value, validateNoteGroup.Value, content);
    }
}