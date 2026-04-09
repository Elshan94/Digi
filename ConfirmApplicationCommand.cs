
using DigitalSalaryService.Models.Common;

namespace DigitalSalaryService.Application.Features.Application.ConfirmApplication
{
    public class ConfirmApplicationCommand : ICommand
    {
        public string RequestId { get; set; } = null!;
    }
}
