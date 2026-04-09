using Ardalis.SmartEnum;

namespace DigitalSalaryService.Persistence.Constants
{
    public class CurrentStep : SmartEnum<CurrentStep>
    {
        public static readonly CurrentStep OrderCreated = new CurrentStep("OrderCreated", 1);
        public static readonly CurrentStep ApplicationConfirmed = new CurrentStep("ApplicationConfirmed", 2);
        public static readonly CurrentStep AsanChecked = new CurrentStep("AsanChecked", 3);

        // KYC
        public static readonly CurrentStep KycPassed = new CurrentStep("KycPassed", 4);
        public static readonly CurrentStep KycExpired = new CurrentStep("KycExpired", 5);
        public static readonly CurrentStep KycCreated = new CurrentStep("KycCreated", 6);

        // FATCA
        public static readonly CurrentStep FatcaPassed = new CurrentStep("FatcaPassed", 7);
        public static readonly CurrentStep FatcaExpired = new CurrentStep("FatcaExpired", 8);
        public static readonly CurrentStep FatcaCreated = new CurrentStep("FatcaCreated", 9);


        // ID document & uploads
        public static readonly CurrentStep IdDocumentChecked = new CurrentStep("IdDocumentChecked", 10);
        public static readonly CurrentStep UploadDataSent = new CurrentStep("UploadDataSent", 11);
        public static readonly CurrentStep VideoRecordResultSuccess = new CurrentStep("VideoRecordResultSuccess", 12);
        public static readonly CurrentStep VideoRecordResultFailed = new CurrentStep("VideoRecordResultFailed", 13);


        // Signing
        public static readonly CurrentStep DocumentSigned = new CurrentStep("DocumentSigned", 14);

        public CurrentStep(string name, int value) : base(name, value)
        {
        }
    }
}
