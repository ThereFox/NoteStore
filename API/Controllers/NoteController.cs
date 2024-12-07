using Application.Commands;
using Application.Commands.Abstraction;
using Application.Requests;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using NoteStore.Domain.ValueObjects;

namespace API.Controllers;

[Controller]
[Route("/notes")]
public class NoteController : Controller
{
    private NoteQueryService _queryService;
    private ICommandHandler<CreateNoteCommand> _createNoteCommandHandler;

    public NoteController(NoteQueryService queryService, ICommandHandler<CreateNoteCommand> createNoteCommandHandler)
    {
        _queryService = queryService;
        _createNoteCommandHandler = createNoteCommandHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateNoteRequest request)
    {
        var validateType = NoteGroup.Create(request.noteGroup);

        if (validateType.IsFailure)
        {
            return BadRequest("Invalid note group");
        }
            
        var command = new CreateNoteCommand()
        {
            group = validateType.Value,
            Content = request.content,
            Descriprion = request.description,
            Header = request.title
        };
        
        var handlerResult = await _createNoteCommandHandler.Handle(command);

        return Ok(handlerResult.IsSuccess);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(Guid id)
    {
        var note = await _queryService.GetById(id);
        if (note.IsFailure)
        {
            return NotFound(note.Error);
        }
        return Ok(note.Value);
    }
    
}