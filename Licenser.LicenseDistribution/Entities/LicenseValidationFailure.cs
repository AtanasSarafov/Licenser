using Standard.Licensing.Validation;

namespace Licenser.LicenseDistribution.Entities
{
    public class LicenseValidationFailure : IValidationFailure
    {
        public string Message { get; set; }
        public string HowToResolve { get; set; }
    }
}
