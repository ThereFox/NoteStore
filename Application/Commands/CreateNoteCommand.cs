using Application.Commands.Abstraction;
using NoteStore.Domain.ValueObjects;

namespace Application.Commands;

public class CreateNoteCommand : ICommand
{
    public Guid Id { get; set; }
    public NoteGroup group  { get; set; }
    public string Header { get; set; }
    public string Descriprion { get; set; }
    public string Content { get; set; }
}