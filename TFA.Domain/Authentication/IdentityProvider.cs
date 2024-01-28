namespace TFA.Domain.Authentication;

internal class IdentityProvider : IIdentityProvider
{
    public IIdentity Current => new User(Guid.Parse("8dc3c631-5277-48f1-b7f0-716083c816fa"));
}
