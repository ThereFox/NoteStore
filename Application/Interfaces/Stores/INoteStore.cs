using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;

namespace Application.Interfaces.Stores;

public interface INoteStore
{
    public Task<IList<Note>> GetAllByGroup(NoteGroup group);
    public IList<Note> GetAllWithRelatedText(string relatedText);
    public Note GetById(Guid id);
}