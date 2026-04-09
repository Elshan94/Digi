using DigitalSalaryService.Application.Features.GetCustomerOrders;
using DigitalSalaryService.Models.PersistanceDTO;
using DigitalSalaryService.Persistence.Entities;

namespace DigitalSalaryService.Persistence.Repositories.Abstract
{
    public interface IOperationRepository : IRepository<SalaryOrder>
    {
        Task<IEnumerable<GetCustomerOrdersResponseDTO>> GetCustomerOrdersAsync(string customerCode, string partnerId, CancellationToken cancellationToken = default);
    }
}
