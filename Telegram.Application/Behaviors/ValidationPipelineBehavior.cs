﻿using FluentValidation;
using MediatR;
using Telegram.Domain.Shared;

namespace Telegram.Application.Behaviors
{
    public class ValidationPipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next();

            var errors = _validators
                .Select(validator => validator.Validate(request))
                .SelectMany(validatorResult => validatorResult.Errors)
                .Where(vFailure => vFailure != null)
                .Select(failure => new Error(
                    failure.PropertyName,
                    failure.ErrorMessage))
                .Distinct()
                .ToArray();

            if (errors.Any()) return CreateValidationResult<TResponse>(errors);

            return await next();
        }

        private static T CreateValidationResult<T>(Error[] errors)
            where T : Result
        {
            if (typeof(T) == typeof(Result))
                return (ValidationResult.WithErrors(errors) as T)!;

            return (T)typeof(ValidationResult<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(typeof(T).GenericTypeArguments[0])
                .GetMethod(nameof(ValidationResult.WithErrors))!
                .Invoke(null, new object?[] { errors })!;
        }
    }
}
