using CSharpFunctionalExtensions;
using NoteStore.Domain;

namespace Application.Interfaces.Stores;

public interface INoteCommandStore
{
    public Task<Result> AddNote(Note note);
    public Task<Result> UpdateNote(Note note);
    public Task<Result> DeleteNote(Guid noteId);
}