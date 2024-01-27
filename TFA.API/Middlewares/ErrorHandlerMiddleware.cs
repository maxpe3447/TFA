using FluentValidation;
using System.Net;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;

namespace TFA.API.Middlewares
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception e)
            {
                var httpCode = e switch
                {
                    IntentionManagerException =>StatusCodes.Status403Forbidden,
                    ValidationException => StatusCodes.Status400BadRequest,
                    DomainException domainException=> domainException.ErrorCode switch
                    {
                        ErrorCode.Gone =>StatusCodes.Status410Gone,
                        _ => StatusCodes.Status500InternalServerError
                    },
                    _ => StatusCodes.Status500InternalServerError
                };
                context.Response.StatusCode = httpCode;

            }
        }
    }
}
