using System.ComponentModel.DataAnnotations;

namespace TFA.Forum.Storage.Entities;

public class DomainEvent
{
    [Key]
    public Guid DomainEventId { get; set; }

    public DateTimeOffset EmittedAt { get; set; }

    [Required]
    public byte[] ContentBlob { get; set; }

    public string? ActivityId { get; set; }
}
