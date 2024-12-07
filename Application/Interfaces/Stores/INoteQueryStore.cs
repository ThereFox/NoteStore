using Application.DTOs;
using CSharpFunctionalExtensions;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;

namespace Application.Interfaces.Stores;

public interface INoteQueryStore
{
    public Task<IList<NoteShortInfo>> GetNByGroup(NoteGroup group, int count);
    public Task<IList<NoteMatch>> GetAllWithRelatedText(string relatedText);
    public Task<Result<Note>> GetById(Guid id);
}