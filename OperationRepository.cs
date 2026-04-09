using DigitalSalaryService.Models.PersistanceDTO;
using DigitalSalaryService.Persistence.Entities;
using DigitalSalaryService.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace DigitalSalaryService.Persistence.Repositories.Concrete
{
    public class OperationRepository : BaseEFRepository<SalaryOrder>, IOperationRepository
    {
        public OperationRepository(DigitalSalaryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GetCustomerOrdersResponseDTO>> GetCustomerOrdersAsync(string customerCode, string partnerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(m => m.CustomerCode == customerCode && m.PartnerId == partnerId).Select(m => new GetCustomerOrdersResponseDTO()
            {
                CurrentStatus = m.CurrentStep,
                RequestId = m.RequestId,
            }).ToListAsync(cancellationToken);
        }
    }
}
