using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MunicipalityManagement.Models
{
    public class Report
    {
        [Key]
        public int ReportID { get; set; }
        [Required] public int CitizenID { get; set; }
        [Required] public string ReportType { get; set; }
        [Required] public string Details { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Under Review";
        [ValidateNever]
        public Citizen Citizen { get; set; } // Navigation property
    }
}
