namespace LeadManagementAPI.DTOs.LeadDtos
{
public class AttachmentResponseDto
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileSizeFormatted { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
