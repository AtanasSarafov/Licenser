using Licenser.LicenseDistribution.Helpers;
using Licenser.WebClient.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Licenser.WebClient.Pages
{
    public class IndexModel : PageModel
    {
        public string HardwareId { get; set; }

        public LicenseViewModel License { get; set; }

        private readonly ILogger<IndexModel> logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            this.logger = logger;
        }

        public void OnGet()
        {
            HardwareId = HardwareInfoHelper.GenerateHardwareId();
        }
    }
}
