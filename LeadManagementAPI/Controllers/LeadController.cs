using LeadManagementAPI.DTOs.LeadDtos;
using LeadManagementAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeadManagementAPI.ServiceExtensions;

namespace LeadManagementAPI.Controllers;

/// <summary>
/// Manages Lead operations including CRUD and attachments
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;
    private readonly ILogger<LeadsController> _logger;

    public LeadsController(ILeadService leadService, ILogger<LeadsController> logger)
    {
        _leadService = leadService;
        _logger = logger;
    }

    /// <summary>
    /// Get all leads with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllLeads([FromQuery] LeadQueryParams query)
    {
        _logger.LogInformation("Fetching leads with filters");

        var result = await _leadService.GetAllLeadsAsync(query);

        return Ok(new ApiResponse<object>(result, "Leads fetched successfully"));
    }

    /// <summary>
    /// Get lead by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetLeadById(int id)
    {
        _logger.LogInformation("Fetching lead with ID {LeadId}", id);

        var lead = await _leadService.GetLeadByIdAsync(id);

        if (lead == null)
            return NotFound(new ApiResponse<object>($"Lead with ID {id} not found"));

        return Ok(new ApiResponse<object>(lead, "Lead fetched successfully"));
    }

    /// <summary>
    /// Create a new lead
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateLead([FromBody] CreateLeadDto dto)
    {
        _logger.LogInformation("Creating new lead");

        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<object>("Validation failed", ModelState));

        var result = await _leadService.CreateLeadAsync(dto);

        return CreatedAtAction(nameof(GetLeadById),
            new { id = result.LeadId },
            new ApiResponse<object>(result, "Lead created successfully"));
    }

    /// <summary>
    /// Update an existing lead
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateLead(int id, [FromBody] UpdateLeadDto dto)
    {
        _logger.LogInformation("Updating lead with ID {LeadId}", id);

        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<object>("Validation failed", ModelState));

        var lead = await _leadService.UpdateLeadAsync(id, dto);

        if (lead == null)
            return NotFound(new ApiResponse<object>($"Lead with ID {id} not found"));

        return Ok(new ApiResponse<object>(lead, "Lead updated successfully"));
    }

    /// <summary>
    /// Delete a lead
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLead(int id)
    {
        _logger.LogInformation("Deleting lead with ID {LeadId}", id);

        var success = await _leadService.DeleteLeadAsync(id);

        if (!success)
            return NotFound(new ApiResponse<object>($"Lead with ID {id} not found"));

        return Ok(new ApiResponse<object>(success, "Lead deleted successfully"));
    }

    /// <summary>
    /// Upload attachments for a lead
    /// </summary>
    [HttpPost("{id:int}/attachments")]
    public async Task<IActionResult> UploadAttachments(int id, [FromForm] List<IFormFile> files)
    {
        _logger.LogInformation("Uploading attachments for lead ID {LeadId}", id);

        if (files == null || files.Count == 0)
            return BadRequest(new ApiResponse<object>("No files provided"));

        var result = await _leadService.UploadAttachmentsAsync(id, files);

        return Ok(new ApiResponse<object>(result, "Files uploaded successfully"));
    }

    /// <summary>
    /// Download attachment
    /// </summary>
    [HttpGet("attachments/{attachmentId:int}/download")]
    public async Task<IActionResult> DownloadAttachment(int attachmentId)
    {
        _logger.LogInformation("Downloading attachment ID {AttachmentId}", attachmentId);

        var result = await _leadService.DownloadAttachmentAsync(attachmentId);

        if (result == null)
            return NotFound(new ApiResponse<object>($"Attachment {attachmentId} not found"));

        var (bytes, contentType, fileName) = result.Value;

        return File(bytes, contentType, fileName);
    }

    /// <summary>
    /// Delete attachment
    /// </summary>
    [HttpDelete("attachments/{attachmentId:int}")]
    public async Task<IActionResult> DeleteAttachment(int attachmentId)
    {
        _logger.LogInformation("Deleting attachment ID {AttachmentId}", attachmentId);

        var success = await _leadService.DeleteAttachmentAsync(attachmentId);

        if (!success)
            return NotFound(new ApiResponse<object>($"Attachment {attachmentId} not found"));

        return Ok(new ApiResponse<object>(success, "Attachment deleted successfully"));
    }
}