namespace LeadManagementAPI.Models;

public enum LeadStatus
{
    New,
    InProgress,
    Converted,
    Lost
}

public class Lead
{
    public int LeadId { get; set; }
    public string LeadName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public LeadStatus Status { get; set; } 
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public ICollection<LeadAttachment> Attachments { get; set; } = new List<LeadAttachment>();
}
