using System.ComponentModel.DataAnnotations.Schema;

namespace TFA.Storage.Entities;

public class Forum
{
    public Guid ForumId { get; set; }
    public string Title { get; set; }

    [InverseProperty(nameof(Topic.Forum))]
    public ICollection<Topic> Topics { get; set; }
}