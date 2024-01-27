using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;

namespace TFA.API.Middlewares
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, ProblemDetailsFactory problemDetailsFactory)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception e)
            {
                var httpCode = e switch
                {
                    IntentionManagerException => StatusCodes.Status403Forbidden,
                    ValidationException => StatusCodes.Status400BadRequest,
                    DomainException domainException => domainException.ErrorCode switch
                    {
                        ErrorCode.Gone => StatusCodes.Status410Gone,
                        _ => StatusCodes.Status500InternalServerError
                    },
                    _ => StatusCodes.Status500InternalServerError
                };
                var problemDetails = e switch
                {
                    IntentionManagerException => problemDetailsFactory.CreateProblemDetails(context, httpCode,
                    "Authorization failed", detail: e.Message),
                    ValidationException => problemDetailsFactory.CreateValidationProblemDetails(context, new ModelStateDictionary(),
                    httpCode, "Invalid request"),
                    DomainException domainException => problemDetailsFactory.CreateProblemDetails(context, httpCode,
                    domainException.Message),
                    _ => problemDetailsFactory.CreateProblemDetails(context, httpCode, "Unhandled error! Please contact us.",
                    detail: e.Message)
                };
                context.Response.StatusCode = httpCode;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
