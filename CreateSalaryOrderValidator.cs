using DigitalSalaryService.Application.Models.Constants;
using FluentValidation;
using System.Text.RegularExpressions;

namespace DigitalSalaryService.Application.Features.CreateSalaryOrder
{
    public class CreateSalaryOrderValidator : AbstractValidator<CreateSalaryOrderCommand>
    {
        public CreateSalaryOrderValidator()
        {
            RuleFor(m => m.CustomerCode).Must(m => Regex.IsMatch(m, "^[0-9]{10}$", RegexOptions.None, TimeSpan.FromSeconds(5))).When(m => !String.IsNullOrEmpty(m.CustomerCode)).WithMessage("Customer code must containts 10 digit numbers");
            RuleFor(x => x.Pin).NotEmpty().Must(m => Regex.IsMatch(m, CustomRegexs.PIN, RegexOptions.None, TimeSpan.FromSeconds(5))).When(m => !String.IsNullOrEmpty(m.Pin)).WithMessage("PIN isn't in correct format");
            RuleFor(x => x.SerialNumber).NotEmpty().Must(m => Regex.IsMatch(m, CustomRegexs.DocumentNumber, RegexOptions.None, TimeSpan.FromSeconds(5))).When(m => !String.IsNullOrEmpty(m.SerialNumber)).WithMessage("Document number isn't in correct format");
        }
    }
}
