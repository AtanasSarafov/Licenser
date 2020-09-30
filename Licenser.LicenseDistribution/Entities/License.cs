using System;
using System.ComponentModel.DataAnnotations;

namespace Licenser.LicenseDistribution.Entities
{
    public class License 
    {
        [Key]
        public long Id { get; set; }

        public DateTime DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public string Name { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int NumberOfConcurrentUserSessionsAllowed { get; set; }

        public string HardwareId { get; set; }

        public bool Active { get; set; }

        public bool InUse { get; set; }

        public string LicenseDocumentId { get; set; }

        public string ValidateDate { get; set; }

        public long GroupId { get; set; }
    }
}
