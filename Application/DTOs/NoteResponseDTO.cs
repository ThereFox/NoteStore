using NoteStore.Domain;

namespace Application.DTOs;

public class NoteResponseDTO
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string CreatorName { get; set; }
    public int GroupId { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
}

public static class ToResponseDTOClass
{
    public static NoteResponseDTO ToResponseDTO(this Note note)
    {
        return new NoteResponseDTO()
        {
            Id = note.Id,
            GroupId = note.Group.Value,
            CreatorId = note.Owner.Id,
            CreatorName = note.Owner.Name,
            Content = note.Content.Content,
            Header = note.Content.Title,
            Description = note.Content.Descriplion
        };
    }
}