using LeadManagementAPI.DTOs.LeadDtos;
using LeadManagementAPI.Models;

namespace LeadManagementAPI.Repositories.Interfaces;

public interface ILeadRepository
{
    Task<PagedResult<Lead>> GetAllAsync(LeadQueryParams query);
    Task<Lead?> GetByIdAsync(int id);
    Task<Lead> CreateAsync(Lead lead);
    Task<Lead> UpdateAsync(Lead lead);
    Task<bool> DeleteAsync(int id);
    Task<List<LeadAttachment>> AddAttachmentsAsync(List<LeadAttachment> attachments);
    Task<LeadAttachment?> GetAttachmentAsync(int attachmentId);
    Task<bool> DeleteAttachmentAsync(int attachmentId);

}
