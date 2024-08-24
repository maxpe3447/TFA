namespace TFA.Forum.Domain.Authentication;

public interface IIdentityProvider
{
    IIdentity Current { get; set; }
}

//public interface IIdentitySetter
//{
//    IIdentity Current { set; }
//}