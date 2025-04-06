using System.ComponentModel.DataAnnotations;

namespace MunicipalityManagement.Models
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string Position { get; set; }
        [Required] public string Department { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string PhoneNumber { get; set; }
        [Required] public DateTime HireDate { get; set; }
    }
}
