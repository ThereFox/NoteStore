namespace Application.Requests;

public sealed record CreateNoteRequest
(
    Guid creatorId,
    int noteGroup,
    string title,
    string description,
    string content
);