using LeadManagementAPI.DTOs.AuthDtos;

namespace LeadManagementAPI.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
