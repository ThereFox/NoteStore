using Application.DTOs;
using Application.Interfaces.Stores;
using CSharpFunctionalExtensions;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;

namespace Application.UseCases;

public class NoteQueryService
{
    private readonly INoteCommandStore _commandStore;
    private readonly INoteQueryStore _queryStore;

    public NoteQueryService(INoteCommandStore commandStore, INoteQueryStore queryStore)
    {
        _commandStore = commandStore;
        _queryStore = queryStore;
    }

    public async Task<Result<Note>> GetById(Guid noteId)
    {
        return await _queryStore.GetById(noteId);
    }
    
    public async Task<Result<IList<NoteShortInfo>>> GetByGroup(int groupId)
    {
        var validateGroup = NoteGroup.Create(groupId);

        if (validateGroup.IsFailure)
        {
            return validateGroup.ConvertFailure<IList<NoteShortInfo>>();
        }
        
        var result = await _queryStore.GetNByGroup(validateGroup.Value, 15);
        
        return Result.Success(result);
    }

    public async Task<Result<IList<NoteMatch>>> GetByRelatedText(string relatedText)
    {
        if (string.IsNullOrWhiteSpace(relatedText))
        {
            return Result.Failure<IList<NoteMatch>>("invalid input");
        }
        
        var result = await _queryStore.GetAllWithRelatedText(relatedText);
        
        return Result.Success(result);
    }

    public async Task<Result> SaveNew()
    {
        
        _commandStore.AddNote();
    }
    
}