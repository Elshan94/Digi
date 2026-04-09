using DigitalSalaryService.Application.Services.Abstract;
using DigitalSalaryService.Persistence.Constants;

namespace DigitalSalaryService.Persistence.Entities
{
    public class SalaryOrder : BaseEntity
    {
        public string RequestId { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CallbackStatus { get; set; }
        public string? ProfileId { get; set; }
        public string? Iban { get; set; }
        public string? FileName { get; set; }
        public DateTime? DocumentSignedDate { get; set; }
        public string? DocumentNumber { get; set; }
        public string? PersonalNumber { get; set; }
        public string? KycId { get; set; }
        public string? FatcaId { get; set; }

        public string ApplicationId { get; set; } = null!;
        public string PartnerId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public DateTime? ApplicationConfirmedDate { get; set; }

        public CurrentStep CurrentStep { get; set; }
        public string CurrentStepLog { get; set; } = string.Empty;

        private static readonly Dictionary<CurrentStep, CurrentStep[]> AllowedTransitions = new()
        {
             { CurrentStep.OrderCreated, new [] { CurrentStep.ApplicationConfirmed } },
              { CurrentStep.ApplicationConfirmed, new [] { CurrentStep.AsanChecked } },
            { CurrentStep.AsanChecked, new [] { CurrentStep.KycPassed, CurrentStep.KycExpired } },

            // KYC
            { CurrentStep.KycPassed, new [] { CurrentStep.FatcaPassed } },
            { CurrentStep.KycExpired, new [] { CurrentStep.KycCreated } },
            { CurrentStep.KycCreated, new [] { CurrentStep.FatcaPassed } },

            // FATCA
            { CurrentStep.FatcaPassed, new [] { CurrentStep.IdDocumentChecked } },
            { CurrentStep.FatcaExpired, new [] { CurrentStep.FatcaCreated } },
            { CurrentStep.FatcaCreated, new [] { CurrentStep.IdDocumentChecked } },

            // ID & Upload
            { CurrentStep.IdDocumentChecked, new [] { CurrentStep.UploadDataSent } },
            { CurrentStep.UploadDataSent, new [] { CurrentStep.VideoRecordResultSuccess, CurrentStep.VideoRecordResultFailed } },
            { CurrentStep.VideoRecordResultFailed, new [] { CurrentStep.DocumentSigned } },
            { CurrentStep.VideoRecordResultSuccess, new [] { CurrentStep.DocumentSigned } },
        };

        public bool TryChangeStep(CurrentStep nextStep)
        {
            if (!AllowedTransitions.TryGetValue(CurrentStep, out var allowed) || !allowed.Contains(nextStep))
                return false;

            CurrentStep = nextStep;

            if (string.IsNullOrEmpty(CurrentStepLog))
                CurrentStepLog = nextStep.Name;
            else
                CurrentStepLog = $"{CurrentStepLog} => {nextStep.Name}";

            return true;
        }


        public bool AsanChecked()
        {
            return TryChangeStep(CurrentStep.AsanChecked);
        }


        public bool KycExpired()
        {
            return TryChangeStep(CurrentStep.KycExpired);
        }


        public bool KycPassed()
        {
            return TryChangeStep(CurrentStep.KycPassed);
        }



        public bool KycCreated()
        {
            return TryChangeStep(CurrentStep.KycCreated);
        }


        public bool FatcaPassed()
        {
            return TryChangeStep(CurrentStep.FatcaPassed);
        }


        public bool FatcaExpired()
        {
            return TryChangeStep(CurrentStep.FatcaExpired);
        }

        public bool FatcaCreated()
        {
            return TryChangeStep(CurrentStep.FatcaCreated);
        }

        public bool ApplicationConfirmed()
        {
            ApplicationConfirmedDate = DateTime.Now;
            return TryChangeStep(CurrentStep.ApplicationConfirmed);
        }

        public bool IdDocumentChecked()
        {
            return TryChangeStep(CurrentStep.IdDocumentChecked);
        }

        public bool UploadDataSent()
        {
            return TryChangeStep(CurrentStep.UploadDataSent);
        }


        public bool MarkVideoRecordResultFailed()
        {
            return TryChangeStep(CurrentStep.VideoRecordResultFailed);
        }


        public bool VideoRecordResultSucceed()
        {
            return TryChangeStep(CurrentStep.VideoRecordResultSuccess);
        }

        public bool DocumentSigned()
        {
            DocumentSignedDate = DateTime.Now;
            return TryChangeStep(CurrentStep.DocumentSigned);
        }

        public void UpdatePersonInfo(string? documentNumber, string? personalNumber, string? customerCode)
        {
            DocumentNumber = documentNumber;
            PersonalNumber = personalNumber;
            CustomerCode = customerCode ?? CustomerCode;
        }

        private static string GenerateRequestId(string customerCode, IDateTimeProvider dateTimeProvider)
        {
            return $"{customerCode}-{dateTimeProvider.CurrentDateTime().ToString("yyyyMMddHHmmssfff")}";
        }

        public static SalaryOrder CreateOrder(string customerCode, string name, string surname, string patronymic, string serialNumber, string pin, string partnerId, string applicationId, IDateTimeProvider dateTimeProvider)
        {
            return new SalaryOrder()
            {
                CurrentStep = CurrentStep.OrderCreated,
                CurrentStepLog = CurrentStep.OrderCreated.Name,
                RequestId = GenerateRequestId(customerCode, dateTimeProvider),
                CustomerCode = customerCode,
                Name = name,
                Surname = surname,
                Patronymic = patronymic,
                DocumentNumber = serialNumber,
                PersonalNumber = pin,
                PartnerId = partnerId,
                ApplicationId = applicationId,
                CreatedDate = dateTimeProvider.CurrentDateTime(),
            };
        }
    }
}
