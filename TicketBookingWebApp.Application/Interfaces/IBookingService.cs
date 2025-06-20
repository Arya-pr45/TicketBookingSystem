using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Application.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(int userId);
        Task<BookingDto?> GetBookingByIdAsync(int bookingId);
        Task<bool> CreateBookingAsync(BookingDto bookingDto);
        Task<bool> CancelBookingAsync(int bookingId);
        //Task SendBookingConfirmationEmail(BookingDto bookingDto);
    }
}
