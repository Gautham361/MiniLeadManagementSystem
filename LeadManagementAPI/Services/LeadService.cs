using LeadManagementAPI.DTOs.LeadDtos;
using LeadManagementAPI.Models;
using LeadManagementAPI.Repositories.Interfaces;
using LeadManagementAPI.Services.Interfaces;

namespace LeadManagementAPI.Services;

/// <summary>
/// Handles business logic for Lead operations
/// </summary>
public class LeadService : ILeadService
{
    private readonly ILeadRepository _repo;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<LeadService> _logger;

    public LeadService(
        ILeadRepository repo,
        IConfiguration config,
        IWebHostEnvironment env,
        ILogger<LeadService> logger)
    {
        _repo = repo;
        _config = config;
        _env = env;
        _logger = logger;
    }

    public async Task<PagedResult<LeadResponseDto>> GetAllLeadsAsync(LeadQueryParams query)
    {
        _logger.LogInformation("Fetching leads with pagination");

        var result = await _repo.GetAllAsync(query);

        return new PagedResult<LeadResponseDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<LeadResponseDto?> GetLeadByIdAsync(int id)
    {
        _logger.LogInformation("Fetching lead with ID {LeadId}", id);

        var lead = await _repo.GetByIdAsync(id);

        return lead == null ? null : MapToDto(lead);
    }

    public async Task<LeadResponseDto> CreateLeadAsync(CreateLeadDto dto)
    {
        _logger.LogInformation("Creating new lead for {LeadName}", dto.LeadName);

        var lead = new Lead
        {
            LeadName = dto.LeadName,
            CompanyName = dto.CompanyName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Status = dto.Status,
            AssignedTo = dto.AssignedTo,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _repo.CreateAsync(lead);

        _logger.LogInformation("Lead created with ID {LeadId}", created.LeadId);

        return MapToDto(created);
    }

    public async Task<LeadResponseDto?> UpdateLeadAsync(int id, UpdateLeadDto dto)
    {
        _logger.LogInformation("Updating lead with ID {LeadId}", id);

        var lead = await _repo.GetByIdAsync(id);
        if (lead == null) return null;

        lead.LeadName = dto.LeadName;
        lead.CompanyName = dto.CompanyName;
        lead.Email = dto.Email;
        lead.PhoneNumber = dto.PhoneNumber;
        lead.Status = dto.Status;
        lead.AssignedTo = dto.AssignedTo;
        lead.UpdatedDate = DateTime.UtcNow;

        var updated = await _repo.UpdateAsync(lead);

        _logger.LogInformation("Lead updated successfully with ID {LeadId}", id);

        return MapToDto(updated);
    }

    public async Task<bool> DeleteLeadAsync(int id)
    {
        _logger.LogInformation("Deleting lead with ID {LeadId}", id);

        var lead = await _repo.GetByIdAsync(id);

        if (lead != null)
        {
            foreach (var att in lead.Attachments)
            {
                DeletePhysicalFile(att.FilePath);
            }
        }

        var deleted = await _repo.DeleteAsync(id);

        _logger.LogInformation("Lead deletion status for ID {LeadId}: {Status}", id, deleted);

        return deleted;
    }

    public async Task<List<AttachmentResponseDto>> UploadAttachmentsAsync(int leadId, List<IFormFile> files)
    {
        _logger.LogInformation("Uploading attachments for lead ID {LeadId}", leadId);

        var lead = await _repo.GetByIdAsync(leadId)
            ?? throw new KeyNotFoundException($"Lead with ID {leadId} not found");

        var uploadPath = GetUploadPath(leadId);
        Directory.CreateDirectory(uploadPath);

        var attachments = new List<LeadAttachment>();

        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadPath, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ✅ NEW OBJECT EVERY LOOP
            attachments.Add(new LeadAttachment
            {
                LeadId = leadId,
                FileName = uniqueName,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                FilePath = filePath
            });
        }

        // ✅ SAVE ALL AT ONCE (CRITICAL FIX)
        var savedList = await _repo.AddAttachmentsAsync(attachments);

        return savedList.Select(a => new AttachmentResponseDto
        {
            Id = a.Id,
            OriginalFileName = a.OriginalFileName
        }).ToList();
    }

    public async Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadAttachmentAsync(int attachmentId)
    {
        _logger.LogInformation("Downloading attachment ID {AttachmentId}", attachmentId);

        var attachment = await _repo.GetAttachmentAsync(attachmentId);

        if (attachment == null || !File.Exists(attachment.FilePath))
            return null;

        var bytes = await File.ReadAllBytesAsync(attachment.FilePath);

        return (bytes, attachment.ContentType, attachment.OriginalFileName);
    }

    public async Task<bool> DeleteAttachmentAsync(int attachmentId)
    {
        _logger.LogInformation("Deleting attachment ID {AttachmentId}", attachmentId);

        var attachment = await _repo.GetAttachmentAsync(attachmentId);

        if (attachment == null) return false;

        DeletePhysicalFile(attachment.FilePath);

        var deleted = await _repo.DeleteAttachmentAsync(attachmentId);

        _logger.LogInformation("Attachment deletion status: {Status}", deleted);

        return deleted;
    }

    // ─── Helpers ─────────────────────────────────────────

    private string GetUploadPath(int leadId)
    {
        var basePath = Path.Combine(
            _env.ContentRootPath,
            _config["FileStorage:UploadPath"] ?? "Uploads");

        return Path.Combine(basePath, leadId.ToString());
    }

    private static void DeletePhysicalFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    private static LeadResponseDto MapToDto(Lead lead) => new()
    {
        LeadId = lead.LeadId,
        LeadName = lead.LeadName,
        CompanyName = lead.CompanyName,
        Email = lead.Email,
        PhoneNumber = lead.PhoneNumber,
        Status = lead.Status.ToString(),
        AssignedTo = lead.AssignedTo,
        CreatedDate = lead.CreatedDate,
        UpdatedDate = lead.UpdatedDate,
        Attachments = lead.Attachments.Select(MapAttachmentToDto).ToList()
    };

    private static AttachmentResponseDto MapAttachmentToDto(LeadAttachment a) => new()
    {
        Id = a.Id,
        LeadId = a.LeadId,
        OriginalFileName = a.OriginalFileName,
        ContentType = a.ContentType,
        FileSize = a.FileSize,
        FileSizeFormatted = FormatFileSize(a.FileSize),
        UploadedAt = a.UploadedAt
    };

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024):F1} MB";
    }
}