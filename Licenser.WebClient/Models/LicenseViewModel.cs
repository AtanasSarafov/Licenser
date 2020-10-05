using System;
using System.ComponentModel.DataAnnotations;

namespace Licenser.WebClient.Models
{
    public class LicenseViewModel
    {
        public long Id { get; set; }

        [Display(Name = "License name")]
        public string Name { get; set; }

        [Display(Name = "Valid from")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "Valid to")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }

        [Display(Name = "Number of concurrent user sessions")]
        public int NumberOfConcurrentUserSessionsAllowed { get; set; }

        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }

        [Display(Name = "Hardware ID")]
        public string HardwareId { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Number of users")]
        public int UsersCount { get; set; }

        [Display(Name = "Active")]
        public bool Imported { get; set; }

        public bool IsExpired { get; set; }
    }
}

