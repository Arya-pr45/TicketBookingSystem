using TicketBookingSystem.Application.DTOs;
using TicketBookingWebApp.Application.DTOs;

namespace TicketBookingWebApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
