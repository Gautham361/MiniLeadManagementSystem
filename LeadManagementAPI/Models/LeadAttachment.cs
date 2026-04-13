namespace LeadManagementAPI.Models;

public class LeadAttachment
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public Lead Lead { get; set; } = null!;
}
