namespace TFA.Forum.Domain.UseCases.SignIn;

public class RecognizedUser
{
    public Guid UserId { get; set; }
    public byte[] Salt { get; set; }
    public byte[] PasswordHash { get; set; }
}
