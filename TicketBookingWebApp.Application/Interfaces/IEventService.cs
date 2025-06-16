using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Application.Services;

using TicketBookingWebApp.Application.DTOs;

namespace TicketBookingWebApp.Application.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<EventDto?> GetEventDetailsAsync(int eventId);

        Task<BookingDto> BookSeatsAsync(int eventId, List<int> seatIds, string username);
        Task<BookingDto> BookGeneralTicketsAsync(int eventId, int quantity, string username);
        Task<BookingDto?> GetBookingByIdAsync(int bookingId);

        Task<IEnumerable<BookingDto>> GetMyBookingsAsync(string username);
        Task<int> CreateEventAsync(EventDto dto);
        Task<EventDto?> GetEventByIdAsync(int id);
        Task UpdateEventAsync(EventDto dto);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<EventDto>> GetUpcomingEventsAsync();
        Task AddSeatsAsync(List<Seat> seats);
        Task CancelBookingAsync(int bookingId, string username);

    }
}
