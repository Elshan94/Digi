using MediatR;
using System.Diagnostics;

namespace DigitalSalaryService.Applicaton.Behaviours
{
    public class LoggingPipelineBehaviour<TRequest, TResponse>(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
       where TRequest : notnull, IRequest<TResponse>
       where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling process started for {@RequestModel}", request);
            var metric = new Stopwatch();
            metric.Start();
            var response = await next();
            metric.Stop();
            var metricTime = metric.Elapsed;
            if (metricTime.Seconds > 5)
                logger.LogWarning("Handling process took to much time. Maybe it needs to be refactored");

            logger.LogInformation("Handling process done in {ProcessTime} seconds and you have response", metricTime.TotalSeconds);
            return response;
        }
    }
}
