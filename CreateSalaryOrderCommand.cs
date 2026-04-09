using DigitalSalaryService.Models.Common;
using System.Windows.Input;

namespace DigitalSalaryService.Application.Features.CreateSalaryOrder
{
    public class CreateSalaryOrderCommand : ICommand<CreateSalaryOrderResponse>
    {
        public string Pin { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string SerialNumber { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public string ApplicationId { get; set; } = null!;
        public string PartnerId { get; set; } = null!;
    }
}
