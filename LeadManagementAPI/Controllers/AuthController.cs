using LeadManagementAPI.DTOs.AuthDtos;
using LeadManagementAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LeadManagementAPI.ServiceExtensions;

namespace LeadManagementAPI.Controllers;

/// <summary>
/// Handles authentication operations like login and registration
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates user and returns JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid login request received");
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(request);

        _logger.LogInformation("User {Username} logged in successfully", request.Username);

        return Ok(new ApiResponse<object>(result, "Login successful"));
    }

}