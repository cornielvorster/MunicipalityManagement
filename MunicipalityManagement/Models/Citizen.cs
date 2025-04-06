using System.ComponentModel.DataAnnotations;

namespace MunicipalityManagement.Models
{
    public class Citizen
    {
        [Key]
        public int CitizenID { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string Address { get; set; }
        [Required] public string PhoneNumber { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
