namespace LeadManagementAPI.DTOs.LeadDtos
{
    public class LeadResponseDto
    {
        public int LeadId { get; set; }
        public string LeadName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<AttachmentResponseDto> Attachments { get; set; } = new();
    }

}
