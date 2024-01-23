namespace TFA.Domain.Identity;

internal interface IIdentityProvider
{
    IIdentity Current { get; }
}
