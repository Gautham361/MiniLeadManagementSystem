namespace LeadManagementAPI.ServiceExtensions;

/// <summary>
/// Standard API response wrapper
/// </summary>
/// <typeparam name="T">Type of response data</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public T? Data { get; set; }
 
    public object? Errors { get; set; }

    public ApiResponse(T data, string message = "Success")
    {
        Success = true;
        Message = message;
        Data = data;
        Errors = null;
    }

    public ApiResponse(string message, object? errors = null)
    {
        Success = false;
        Message = message;
        Data = default;
        Errors = errors;
    }
}