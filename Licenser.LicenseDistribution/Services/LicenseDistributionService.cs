using Licenser.LicenseDistribution.Constants;
using Licenser.LicenseDistribution.Entities;
using Licenser.LicenseDistribution.Extensions;
using Licenser.LicenseDistribution.Helpers;
using Standard.Licensing;
using Standard.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using License = Standard.Licensing.License;

namespace Licenser.LicenseDistribution.Services
{
    public class LicenseDistributionService : ILicenseDistributionService
    {
        private readonly ILicenseKeyService licenseKeyService;

        public LicenseDistributionService(ILicenseKeyService licenseKeyService)
        {
            this.licenseKeyService = licenseKeyService;
        }

        public async Task<License> LoadLicenseFromDisk(string licenseDocumentId)
        {
            var filePath = $"{LicenseConstants.LicensesDirectory}/{licenseDocumentId}{LicenseConstants.LicensFileExtension}";
            if (!File.Exists(filePath))
            {
                return null;
            }

            return License.Load(await File.ReadAllTextAsync(filePath));
        }

        public License LoadLicense(string xmlString)
        {
            return License.Load(xmlString);
        }

        public License LoadLicense(Stream stream)
        {
            return License.Load(stream);
        }

        public async Task<License> CreateLicense(Entities.License license, string customerName, string contactEmail)
        {
            var additionalAttributes = new Dictionary<string, string>
            {
                {nameof(license.Name), license.Name },
                {nameof(license.CreatedBy), license.CreatedBy },
                {nameof(license.DateFrom), license.DateFrom.ToString() },
                {nameof(license.HardwareId), HardwareInfoHelper.EncryptHardwareId(license.HardwareId)},
                {nameof(license.GroupId), license.GroupId.ToString() },
                {nameof(license.Active), license.Active.ToString() },
                {LicenseConstants.LicenseDbIdAttributeName, license.Id.ToString() }
            };

            var productFeatures = new Dictionary<string, string>
            {
                {nameof(license.NumberOfConcurrentUserSessionsAllowed), license.NumberOfConcurrentUserSessionsAllowed.ToString()},
            };

            return License.New()
                .WithUniqueIdentifier(Guid.NewGuid())
                .As(LicenseType.Trial)
                .ExpiresAt(license.DateTo)
                .WithMaximumUtilization(license.NumberOfConcurrentUserSessionsAllowed)
                .WithProductFeatures(productFeatures)
                .WithAdditionalAttributes(additionalAttributes)
                .LicensedTo(customerName, contactEmail)
                .CreateAndSignWithPrivateKey(await this.licenseKeyService.GetPrivateKey(), LicenseConstants.PassPhrase);
        }

        public async Task<IEnumerable<LicenseValidationFailure>> ValidateLicense(License license, string customerName, string contactEmail, DateTime validationDate)
        {
            return license.Validate()
                          .ExpirationDate()
                          .When(lic => lic.Type == LicenseType.Trial)
                          .And()
                          .IsLicensedTo(customerName, contactEmail)
                          .And()
                          .HasValidHardwareId()
                          .And()
                          .HasValidValidationDate(validationDate)
                          .And()
                          .Signature(await this.licenseKeyService.GetPublicKey())
                          .AssertValidLicense()
                          .Select(i => new LicenseValidationFailure()
                          {
                              Message = i.Message,
                              HowToResolve = i.HowToResolve
                          });
        }

        public async Task SaveLicense(License license)
        {
            Directory.CreateDirectory(LicenseConstants.LicensesDirectory);
            var fileName = license.Id.ToString();
            await File.WriteAllTextAsync($"{LicenseConstants.LicensesDirectory}/{fileName}{LicenseConstants.LicensFileExtension}", license.ToString(), Encoding.UTF8);
        }

        public void DeleteLicense(License license)
        {
            var fileName = license.Id.ToString();
            File.Delete($"{LicenseConstants.LicensesDirectory}/{fileName}{LicenseConstants.LicensFileExtension}");
        }
    }
}
