namespace TFA.API.IAuthentication;

public interface IAuthTokenStorage
{
    bool TryExtract(HttpContext context, out string token);
    void Store(HttpContext context, string token);
}

internal class AuthTokenStorage : IAuthTokenStorage
{
    private const string HeaderKey = "TFA-Auth-Token";
    public void Store(HttpContext context, string token)
    {
        context.Response.Headers[HeaderKey] = token;
    }

    public bool TryExtract(HttpContext context, out string token)
    {
        if(context.Request.Headers.TryGetValue(HeaderKey, out var values) && !string.IsNullOrWhiteSpace(values.FirstOrDefault()))
        {
            token = values.First()!;
            return true;
        }

        token = string.Empty;
        return false;

    }
}
