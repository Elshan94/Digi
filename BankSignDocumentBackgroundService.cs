
using DigitalSalaryService.Application.Models.IB;
using DigitalSalaryService.Application.Services.Abstract;
using DigitalSalaryService.Persistence.Constants;
using DigitalSalaryService.Persistence.Repositories.Abstract;

namespace DigitalSalaryService.Application.Services.Concrete
{
    public class BankSignDocumentBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BankSignDocumentBackgroundService> _logger;

        public BankSignDocumentBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<BankSignDocumentBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bank document signing background service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var documentSigningService =
                        scope.ServiceProvider.GetRequiredService<IDocumentSigningService>();

                    await documentSigningService.SignDocumentsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing document signing operations.");
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            _logger.LogInformation("Bank document signing background service stopped.");
        }
    }
    
}
