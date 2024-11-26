namespace Persistense.Common.Cassandra.DTOs;

public class NoteDTO
{
    public Guid Id { get; set; }
    public int PartitionId { get; set; }
    public string ContentId { get; set; }
    public Guid CreatorId { get; set; }
    public string CreatorName { get; set; }
}