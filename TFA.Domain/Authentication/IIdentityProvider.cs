namespace TFA.Domain.Authentication;

public interface IIdentityProvider
{
    IIdentity Current { get; }
}

public interface IIdentitySetter
{
    IIdentity Current { set; }
}