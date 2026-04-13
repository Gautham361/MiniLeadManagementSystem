namespace LeadManagementAPI.DTOs.LeadDtos
{
    public class LeadQueryParams
    {
        public string? Search { get; set; }
        public List<string>? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }
    }
}
