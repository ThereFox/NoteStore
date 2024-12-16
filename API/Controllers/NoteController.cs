using Application.Commands;
using Application.Commands.Abstraction;
using Application.Requests;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using NoteStore.Domain.ValueObjects;

namespace API.Controllers;

[Controller]
[Route("/")]
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
            Id = Guid.NewGuid(),
            group = validateType.Value,
            Content = request.content,
            Descriprion = request.description,
            Header = request.title
        };
        
        var handlerResult = await _createNoteCommandHandler.Handle(command);

        return Ok(command.Id);
    }

    
    [HttpGet]
    [Route("/")]
    public async Task<IActionResult> GetDefault()
    {
        var getElementsByGroupResult = await _queryService.GetByGroup(0);

        if (getElementsByGroupResult.IsFailure)
        {
            return BadRequest(getElementsByGroupResult.Error);
        }
        return Json(getElementsByGroupResult.Value);
    }
    
    [HttpGet]
    [Route("{groupId}")]
    public async Task<IActionResult> GetByGroup(int groupId)
    {
        var getElementsByGroupResult = await _queryService.GetByGroup(groupId);

        if (getElementsByGroupResult.IsFailure)
        {
            return BadRequest(getElementsByGroupResult.Error);
        }
        return Json(getElementsByGroupResult.Value);
    }
    
    [HttpGet]
    [Route("byId/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var note = await _queryService.GetById(id);
        if (note.IsFailure)
        {
            return NotFound(note.Error);
        }
        return Ok(note.Value);
    }
    
    [HttpGet]
    [Route("bytext/{text}")]
    public async Task<IActionResult> GetRelative(string text)
    {
        var note = await _queryService.GetByRelatedText(text);
        if (note.IsFailure)
        {
            return NotFound(note.Error);
        }
        return Ok(note.Value);
    }
    
}