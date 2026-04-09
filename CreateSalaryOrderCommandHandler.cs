using CSharpFunctionalExtensions;
using DigitalSalaryService.Application.Services.Abstract;
using DigitalSalaryService.Models.Common;
using DigitalSalaryService.Persistence.Constants;
using DigitalSalaryService.Persistence.Entities;
using DigitalSalaryService.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Buffers;

namespace DigitalSalaryService.Application.Features.CreateSalaryOrder
{
    public class CreateSalaryOrderCommandHandler(IOperationRepository operationRepository, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider, ILogger<CreateSalaryOrderCommandHandler> logger) : ICommandHandler<CreateSalaryOrderCommand, CreateSalaryOrderResponse>
    {
        public async Task<Result<CreateSalaryOrderResponse, ErrorModel>> Handle(CreateSalaryOrderCommand request, CancellationToken cancellationToken)
        {
            using var scope = logger.BeginScope(
            "CreateSalaryOrderCommand CustomerCode:{CustomerCode} PartnerId:{PartnerId}",
            request.CustomerCode,
            request.PartnerId);

            logger.LogInformation("Create salary order process started.");

            var existingSalaryOrder = await operationRepository
                .GetAll()
                .OrderByDescending(x=>x.CreatedDate)
                .FirstOrDefaultAsync(x => x.CustomerCode == request.CustomerCode && x.PartnerId == request.PartnerId, cancellationToken);

            if (existingSalaryOrder != null)
            {
                logger.LogInformation("Salary order already exists. Returning existing. RequestId:{RequestId}", existingSalaryOrder.RequestId);
                return Result.Success<CreateSalaryOrderResponse, ErrorModel>(new CreateSalaryOrderResponse
                {
                    RequestId = existingSalaryOrder.RequestId,
                    CurrentStep = existingSalaryOrder.CurrentStep.Name,
                });
            }

            logger.LogInformation("Creating new salary order.");
            var salaryOrder = SalaryOrder.CreateOrder(
                request.CustomerCode,
                request.Name,
                request.Surname,
                request.Patronymic,
                request.SerialNumber,
                request.Pin,
                request.PartnerId,
                request.ApplicationId,
                dateTimeProvider);

            await operationRepository.AddAsync(salaryOrder, cancellationToken); 
            await unitOfWork.SaveEntitiesAsync(cancellationToken);

            logger.LogInformation("Salary order created successfully. RequestId:{RequestId}", salaryOrder.RequestId);

            return Result.Success<CreateSalaryOrderResponse, ErrorModel>(new CreateSalaryOrderResponse
            {
                CurrentStep = salaryOrder.CurrentStep.Name, 
                RequestId = salaryOrder.RequestId
            });

        }
    }
}
