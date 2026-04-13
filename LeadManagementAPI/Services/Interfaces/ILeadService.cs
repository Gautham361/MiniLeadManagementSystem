using LeadManagementAPI.DTOs.LeadDtos;

namespace LeadManagementAPI.Services.Interfaces;

public interface ILeadService
{
    Task<PagedResult<LeadResponseDto>> GetAllLeadsAsync(LeadQueryParams query);
    Task<LeadResponseDto?> GetLeadByIdAsync(int id);
    Task<LeadResponseDto> CreateLeadAsync(CreateLeadDto dto);
    Task<LeadResponseDto?> UpdateLeadAsync(int id, UpdateLeadDto dto);
    Task<bool> DeleteLeadAsync(int id);
    Task<List<AttachmentResponseDto>> UploadAttachmentsAsync(int leadId, List<IFormFile> files);
    Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadAttachmentAsync(int attachmentId);
    Task<bool> DeleteAttachmentAsync(int attachmentId);
}
