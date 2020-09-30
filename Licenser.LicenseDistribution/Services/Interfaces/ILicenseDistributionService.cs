using Licenser.LicenseDistribution.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using License = Standard.Licensing.License;

namespace Licenser.LicenseDistribution.Services
{
    public interface ILicenseDistributionService
    {
        Task<License> LoadLicenseFromDisk(string licenseDocumentId);

        License LoadLicense(string xmlString);

        License LoadLicense(Stream stream);

        Task<License> CreateLicense(Entities.License license, string customerName, string contactEmail);

        Task<IEnumerable<LicenseValidationFailure>> ValidateLicense(License license, string customerName, string contactEmail, DateTime validationDate);

        Task SaveLicense(License license);

        void DeleteLicense(License license);
    }
}
