namespace TFA.Domain.Authentication;

public class BasicSignInCredentials
{
    public string Login { get; set; }
    public string Password { get; set; }

    public BasicSignInCredentials(string login, string password)
    {
        Login = login;
        Password = password;
    }

}
