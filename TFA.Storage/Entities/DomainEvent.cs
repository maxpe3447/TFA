using System.ComponentModel.DataAnnotations;

public class DomainEvent
{
    [Key]
    public Guid DomainEventId { get; set; }

    public DateTimeOffset EmittedAt { get; set; }

    [Required]
    public byte[] ContentBlob { get; set; }
}