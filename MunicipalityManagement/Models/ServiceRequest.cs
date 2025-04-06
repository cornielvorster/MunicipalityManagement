using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MunicipalityManagement.Models
{
    public class ServiceRequest
    {
        [Key]
        public int RequestID { get; set; }
        [Required] public int CitizenID { get; set; }
        [Required] public string ServiceType { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        [ValidateNever]
        public Citizen Citizen { get; set; } // Navigation property
    }
}
