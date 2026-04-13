using LeadManagementAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace LeadManagementAPI.DTOs.LeadDtos
{
    public class CreateLeadDto
    {
        [Required, MaxLength(200)] 
        public string LeadName { get; set; } = string.Empty;

        [Required, MaxLength(200)] 
        public string CompanyName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(200)] 
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)] 
        public string PhoneNumber { get; set; } = string.Empty;

        public LeadStatus Status { get; set; } 

        [Required, MaxLength(200)] 
        public string AssignedTo { get; set; } = string.Empty;
    }
}
