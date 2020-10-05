using Licenser.LicenseDistribution.Entities;
using Licenser.LicenseDistribution.Services;
using Licenser.WebClient.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licenser.WebClient.Controllers
{
    public class LicensesController : Controller
    {
        private readonly ILicenseDistributionService licenseDistributionService;

        public LicensesController(ILicenseDistributionService licenseDistributionService)
        {
            this.licenseDistributionService = licenseDistributionService;
        }

        [HttpPost]
        public async Task<IActionResult> Export(LicenseViewModel model)
        {
            // TODO: Add validations.
            try
            {
                var license = this.CreateNewLicense(model);
                var licenseFileData = await licenseDistributionService.CreateLicense(license, "customerName", "contactEmail");
                var licenseFileName = $"{license.Name}_{license.HardwareId}.lic";

                return Json(new { data = Encoding.Unicode.GetBytes(licenseFileData.ToString()), type = "application/x-enterlicense", fileName = licenseFileName });
                //  return File(Encoding.Unicode.GetBytes(licenseFileData.ToString()), "application/x-enterlicense", licenseFileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file, long groupId)
        {
            if (file == null)
            {
                return BadRequest();
            }

            try
            {
                var licenseDocument = this.GetLicenseDocument(file);
                if (licenseDocument == null)
                {
                    return BadRequest(licenseDocument);
                }

                try
                {
                    var licenseValidationMessages = (await this.licenseDistributionService.ValidateLicense(licenseDocument, "customerName", "contactEmail", new DateTime())).ToList();
                    if (licenseValidationMessages.Any())
                    {
                        return Json(new { licenseValidationMessages });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { licenseValidationMessages = ex });
                }

                await this.licenseDistributionService.SaveLicense(licenseDocument);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private License CreateNewLicense(LicenseViewModel model)
        {
            var newLicense = new License()
            {
                Name = model.Name,
                DateFrom = model.DateFrom,
                DateTo = model.DateTo,
                NumberOfConcurrentUserSessionsAllowed = model.NumberOfConcurrentUserSessionsAllowed,
                HardwareId = model.HardwareId,
                Active = true
            };

            return newLicense;
        }

        private Standard.Licensing.License GetLicenseDocument(IFormFile file)
        {
            using (var ms = file.OpenReadStream())
            {
                return licenseDistributionService.LoadLicense(ms);
            }
        }
    }
}