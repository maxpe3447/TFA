namespace TFA.Forum.Storage;

internal interface IGuidFactory
{
    public Guid Create();
}
internal class GuidFactory : IGuidFactory
{
    public Guid Create() => Guid.NewGuid();
}
