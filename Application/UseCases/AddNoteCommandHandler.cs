using Application.Commands;
using Application.Commands.Abstraction;
using Application.Interfaces.Stores;
using CSharpFunctionalExtensions;
using NoteStore.Domain;
using NoteStore.Domain.ValueObjects;

namespace Application.UseCases;

public class AddNoteCommandHandler : ICommandHandler<CreateNoteCommand>
{
    private readonly INoteCommandStore _store;

    public AddNoteCommandHandler(INoteCommandStore store)
    {
        _store = store;
    }
    
    public async Task<Result> Handle(CreateNoteCommand command)
    {
        var validateContent = NoteContent.Create(DateTime.Now, command.Header, command.Descriprion, command.Content);

        if (validateContent.IsFailure)
        {
            return validateContent;
        }

        var validateOwner = NoteOwner.Create(Guid.NewGuid(), "Test");

        if (validateOwner.IsFailure)
        {
            return validateOwner;
        }
        
        var validateNote = Note.Create(command.Id, validateOwner.Value, command.group, validateContent.Value);

        if (validateNote.IsFailure)
        {
            return validateNote;
        }
        
        return await _store.AddNote(validateNote.Value);
    }
}