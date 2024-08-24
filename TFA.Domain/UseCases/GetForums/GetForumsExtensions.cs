using TFA.Forum.Domain.Exceptions;

namespace TFA.Forum.Domain.UseCases.GetForums;

internal static class GetForumsExtensions
{
    private static async Task<bool> ForumExists(this IGetForumsStorage storage, Guid forumId, CancellationToken cancellationToken)
    {
        var forums = await storage.GetForums(cancellationToken);
        return forums.Any(f => f.Id == forumId);
    }

    public static async Task ThrowIfForumNotFound(this IGetForumsStorage storage, Guid forumId, CancellationToken cancellationToken)
    {
        if (!await storage.ForumExists(forumId, cancellationToken))
        {
            throw new ForumNotFoundException(forumId);
        }
    }
}
