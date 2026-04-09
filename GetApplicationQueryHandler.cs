using CSharpFunctionalExtensions;
using DigitalSalaryService.Application.Services.Abstract;
using DigitalSalaryService.Models.Common;
using DigitalSalaryService.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DigitalSalaryService.Application.Features.Application.GetApplication
{
    public class GetApplicationQueryHandler(IOperationRepository operationRepository, IApplicationService applicationService, ILogger<GetApplicationQueryHandler> logger) : IQueryHandler<GetApplicationQuery, GetApplicationQueryResponse>
    {
        public async Task<Result<GetApplicationQueryResponse, ErrorModel>> Handle(GetApplicationQuery request, CancellationToken cancellationToken)
        {
            using var scope = logger.BeginScope("GetApplicationQuery RequestId:{RequestId}", request.RequestId);

            logger.LogInformation("GetApplicationQuery started");

            var salaryOrder = await operationRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.RequestId == request.RequestId);

            if (salaryOrder == null)
            {
                logger.LogWarning("No request found");
                return Result.Failure<GetApplicationQueryResponse, ErrorModel>(ErrorModel.Create("No request found!"));
            }

            string fullName = $"{salaryOrder.Name} {salaryOrder.Surname} {salaryOrder.Patronymic}";

            var consentResult = applicationService.GetApplication(fullName);

            if (consentResult.IsFailure)
            {
                return Result.Failure<GetApplicationQueryResponse, ErrorModel>(ErrorModel.Create("Consent not found!"));
            }

            GetApplicationQueryResponse queryResponse = new GetApplicationQueryResponse
            {
                PersonalData = consentResult.Value.personConsent,
                AsanFinanceFile = consentResult.Value.asanConsent,
            };

            logger.LogInformation("GetApplicationQuery succeeded");

            return Result.Success<GetApplicationQueryResponse, ErrorModel>(queryResponse);
        }
    }
}
