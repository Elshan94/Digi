using DigitalSalaryService.Models.Common;

namespace DigitalSalaryService.Application.Features.Application.GetApplication
{
    public class GetApplicationQuery : IQuery<GetApplicationQueryResponse>
    {
        public string RequestId { get; set; } = null!;
    }
}
