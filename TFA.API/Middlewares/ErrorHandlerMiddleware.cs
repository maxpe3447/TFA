using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
                var problemDetails = e switch
                {
                    IntentionManagerException intentionManagerException =>
                        problemDetailsFactory.CreateFrom(context, intentionManagerException),
                    ValidationException validationException =>
                        problemDetailsFactory.CreateFrom(context, validationException),
                    DomainException domainException =>
                        problemDetailsFactory.CreateFrom(context, domainException),

                    _ => problemDetailsFactory.CreateProblemDetails(context, StatusCodes.Status500InternalServerError, "Unhandled error! Please contact us.",
                    detail: e.Message)
                };

                context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                context.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType());
            }
        }
    }
}
