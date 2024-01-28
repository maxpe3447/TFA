﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;

namespace TFA.API.Middlewares
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(
            HttpContext context,
            ILogger<ErrorHandlerMiddleware> logger,
            ProblemDetailsFactory problemDetailsFactory)
        {
            try
            {
                logger.LogError("Error handling started for request in path {RequestPath}", context.Request.Path.Value);

                await next.Invoke(context);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Error has happened with {RequestPath}, the message is {ErrorMessage}",
                    context.Request.Path.Value, exception.Message);

                ProblemDetails problemDetails;

                switch (exception)
                {
                    case IntentionManagerException intentionManagerException:
                        problemDetails = problemDetailsFactory.CreateFrom(context, intentionManagerException);
                        break;
                    case ValidationException validationException:
                        problemDetails = problemDetailsFactory.CreateFrom(context, validationException);
                        logger.LogInformation(validationException, "Somebody sent invalid request, oops");
                        break;
                    case DomainException domainException:
                        problemDetails = problemDetailsFactory.CreateFrom(context, domainException);
                        logger.LogError(domainException, "Domain exception occurred");
                        break;
                    default:
                        problemDetails = problemDetailsFactory.CreateProblemDetails(
                            context, StatusCodes.Status500InternalServerError, "Unhandled error! Please contact us.");
                        logger.LogError(exception, "Unhandled exception occurred");
                        break;
                }


                context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType());
            }
        }
    }
}
