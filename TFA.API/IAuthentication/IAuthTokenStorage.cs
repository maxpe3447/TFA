using System.Diagnostics;

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
        //context.Response.Headers[HeaderKey]= token;

        context.Response.Cookies.Append(HeaderKey, token//, new CookieOptions()
        /*{
            HttpOnly = true
        }*/);
    }

    public bool TryExtract(HttpContext context, out string token)
    {
        Debug.WriteLine(string.Join('\n', context.Request.Cookies.Select(x=>$"{x.Key} {x.Value}").ToList()));

        if (context.Request.Cookies.TryGetValue(HeaderKey, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            token = value;
            return true;
        }

        //if (context.Request.Headers.TryGetValue(HeaderKey, out var values) && !string.IsNullOrWhiteSpace(values.FirstOrDefault()))
        //{
        //    token = values.First()!;
        //    return true;
        //}

        token = string.Empty;
        return false;

    }
}
