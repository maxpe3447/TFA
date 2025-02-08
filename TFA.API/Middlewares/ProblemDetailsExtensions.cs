using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TFA.Forum.Domain.Authorization;
using TFA.Forum.Domain.Exceptions;

namespace TFA.API.Middlewares;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails CreateFrom(this ProblemDetailsFactory factory,
        HttpContext context,
        IntentionManagerException intentionManagerException) =>
        factory.CreateProblemDetails(context,
            StatusCodes.Status403Forbidden,
            "Authorization failed",
            detail: intentionManagerException.Message);

    public static ProblemDetails CreateFrom(this ProblemDetailsFactory factory,
        HttpContext context,
        DomainException domainException) =>
        factory.CreateProblemDetails(context,
            domainException.DomainErrorCode switch
            {
                DomainErrorCode.Gone => StatusCodes.Status410Gone,
                _ => StatusCodes.Status500InternalServerError
            },
            "Authorization failed",
            detail: domainException.Message);

    public static ProblemDetails CreateFrom(this ProblemDetailsFactory factory,
    HttpContext context,
    ValidationException validationException)
    {
        var model = new ModelStateDictionary();

        foreach (var error in validationException.Errors)
        {
            model.AddModelError(error.PropertyName, error.ErrorCode);
        }

        return factory.CreateValidationProblemDetails(context, model, StatusCodes.Status400BadRequest);
    }
}
