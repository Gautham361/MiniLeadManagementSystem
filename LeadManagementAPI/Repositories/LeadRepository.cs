using LeadManagementAPI.Data;
using LeadManagementAPI.DTOs.LeadDtos;
using LeadManagementAPI.Models;
using LeadManagementAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeadManagementAPI.Repositories;

/// <summary>
/// Handles database operations for Leads
/// </summary>
public class LeadRepository : ILeadRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LeadRepository> _logger;

    public LeadRepository(ApplicationDbContext context, ILogger<LeadRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<Lead>> GetAllAsync(LeadQueryParams query)
    {
        _logger.LogInformation("Fetching leads from database");

        var leadsQuery = _context.Leads.AsNoTracking().Include(l => l.Attachments).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            if (int.TryParse(search, out var leadId))
            {
                leadsQuery = leadsQuery.Where(l => l.LeadId == leadId);
            }
            else
            {
                leadsQuery = leadsQuery.Where(l =>
                    l.LeadName.Contains(search) ||
                    l.CompanyName.Contains(search) ||
                    l.Email.Contains(search));
            }
        }

        if (query.Status != null && query.Status.Any())
        {
            var validStatuses = new List<LeadStatus>();

            foreach (var status in query.Status)
            {
                if (Enum.TryParse<LeadStatus>(status, true, out var parsed))
                {
                    validStatuses.Add(parsed);
                }
            }

            if (validStatuses.Any())
            {
                leadsQuery = leadsQuery.Where(l => validStatuses.Contains(l.Status));
            }
        }   

        var totalCount = await leadsQuery.CountAsync();

        var items = await leadsQuery.OrderByDescending(l => l.CreatedDate).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();

        return new PagedResult<Lead>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<Lead?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching lead with ID {LeadId}", id);

        return await _context.Leads.AsNoTracking().Include(l => l.Attachments).FirstOrDefaultAsync(l => l.LeadId == id);
    }

    public async Task<Lead> CreateAsync(Lead lead)
    {
        _logger.LogInformation("Creating lead in database");

        await _context.Leads.AddAsync(lead);
        await _context.SaveChangesAsync();

        return lead;
    }

    public async Task<Lead> UpdateAsync(Lead lead)
    {
        _logger.LogInformation("Updating lead with ID {LeadId}", lead.LeadId);

        _context.Leads.Update(lead);
        await _context.SaveChangesAsync();

        return lead;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting lead with ID {LeadId}", id);

        var lead = await _context.Leads.FindAsync(id);
        if (lead == null) return false;

        _context.Leads.Remove(lead);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<LeadAttachment>> AddAttachmentsAsync(List<LeadAttachment> attachments)
    {
        _context.LeadAttachments.AddRange(attachments); 
        await _context.SaveChangesAsync();              
        return attachments;
    }

    public async Task<LeadAttachment?> GetAttachmentAsync(int attachmentId)
    {
        _logger.LogInformation("Fetching attachment ID {AttachmentId}", attachmentId);

        return await _context.LeadAttachments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attachmentId);
    }

    public async Task<bool> DeleteAttachmentAsync(int attachmentId)
    {
        _logger.LogInformation("Deleting attachment ID {AttachmentId}", attachmentId);

        var attachment = await _context.LeadAttachments.FindAsync(attachmentId);
        if (attachment == null) return false;

        _context.LeadAttachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return true;
    }
}