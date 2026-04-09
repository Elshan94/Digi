using CSharpFunctionalExtensions;
using DigitalSalaryService.Models.Common;
using DigitalSalaryService.Persistence.Repositories.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalSalaryService.Application.Features.Application.ConfirmApplication
{
    public class ConfirmApplicationCommandHandler(IOperationRepository operationRepository, IUnitOfWork unitOfWork, ILogger<ConfirmApplicationCommandHandler> logger) : ICommandHandler<ConfirmApplicationCommand>
    {
        public async Task<Result<Unit, ErrorModel>> Handle(ConfirmApplicationCommand request, CancellationToken cancellationToken)
        {
            using var scope = logger.BeginScope("ConfirmApplicationCommand RequestId:{RequestId}", request.RequestId);

            var salaryOrder = await operationRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.RequestId == request.RequestId);

            if (salaryOrder == null)
            {
                logger.LogWarning("No order found for RequestId:{RequestId}", request.RequestId);
                return Result.Failure<Unit, ErrorModel>(ErrorModel.Create("No order found!"));
            }

            bool transitionResult = salaryOrder.ApplicationConfirmed();
            if (!transitionResult)
            {
                logger.LogError("Invalid workflow transition to ApplicationConfirmed");
                return Result.Failure<Unit, ErrorModel>(
                    ErrorModel.Create("Invalid workflow transition to ApplicationConfirmed."));
            }

            logger.LogInformation("Salary order marked as ApplicationConfirmed");
            operationRepository.Update(salaryOrder);
            await unitOfWork.SaveEntitiesAsync(cancellationToken);
            logger.LogInformation("Salary order update saved successfully ");

            return Result.Success<Unit, ErrorModel>(Unit.Value);
        }
    }
}
