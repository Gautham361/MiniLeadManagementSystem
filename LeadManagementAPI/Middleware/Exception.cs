namespace LeadManagementAPI.Middleware
{
    /// <summary>
    /// Custom exception for not found cases
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
