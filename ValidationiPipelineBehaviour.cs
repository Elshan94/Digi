using DigitalSalaryService.Models.Common;
using FluentValidation;
using MediatR;

namespace DigitalDepositAccountService.Applicaton.Behaviours
{
    public class ValidationiPipelineBehaviour<TRequest, TResponse>
      (IEnumerable<IValidator<TRequest>> validators)
      : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequestBase
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
                return await next();

            var validationContext = new ValidationContext<TRequest>(request);
            var validationResponse = await Task.WhenAll(validators.Select(x => x.ValidateAsync(validationContext)));

            var validationErrors = validationResponse.Where(x => x.Errors.Any())
                .SelectMany(x => x.Errors)
                .ToList();

            if (validationErrors.Any())
                throw new ValidationException(validationErrors);

            return await next();
        }
    }
}
