using Licenser.LicenseDistribution.Helpers;
using Standard.Licensing.Validation;
using System;
using License = Standard.Licensing.License;

namespace Licenser.LicenseDistribution.Extensions
{
    internal static class LicenseValidationExtensions
    {
        internal static IValidationChain IsLicensedTo(this IStartValidationChain validationChain, string customerName, string contactEmail)
        {
            return validationChain.AssertThat(license => CheckCustomer(license, customerName, contactEmail),
                new GeneralValidationFailure()
                {
                    Message = "Wrong license file!",
                    HowToResolve = "Please contact the system administrator!"
                });
        }

        internal static IValidationChain HasValidHardwareId(this IStartValidationChain validationChain)
        {
            return validationChain.AssertThat(license => CheckHardwareId(license),
                new GeneralValidationFailure()
                {
                    Message = "The Hardware ID of the current machine is not licensed!",
                    HowToResolve = "Please contact the system administrator!"
                });
        }

        internal static IValidationChain HasValidValidationDate(this IStartValidationChain validationChain, DateTime validationDate)
        {
            return validationChain.AssertThat(license => CheckValidationDate(validationDate),
                new GeneralValidationFailure()
                {
                    Message = "System date-time changes detected! Please import extended license file or contact the system administrator!",
                    HowToResolve = "Please contact the system administrator!"
                });
        }

        private static bool CheckCustomer(License license, string customerName, string contactEmail)
        {
            if (license.Customer == null || string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(contactEmail))
            {
                return false;
            }

            return license.Customer.Name == customerName
                && license.Customer.Email == contactEmail;
        }

        private static bool CheckHardwareId(License license)
        {
            // TODO: Export this string value.
            var licenseHardwareId = license.AdditionalAttributes.Get("HardwareId");
            if (string.IsNullOrEmpty(licenseHardwareId))
            {
                return false;
            }

            return HardwareInfoHelper.ValidateHardwareId(licenseHardwareId);
        }

        private static bool CheckValidationDate(DateTime validateDate)
        {
            return validateDate <= DateTime.Now;
        }
    }
}
