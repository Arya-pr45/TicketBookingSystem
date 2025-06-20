using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Application.Services;

using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Domain.Enums;

namespace TicketBookingWebApp.Application.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<EventDto?> GetEventDetailsAsync(int eventId);

        Task<BookingDto> BookSeatsAsync(int eventId, List<int> seatIds, User user);
        Task<BookingDto> BookGeneralTicketsAsync(int eventId, int quantity, User user);
        Task<BookingDto?> GetBookingByIdAsync(int bookingId);

        Task<IEnumerable<BookingDto>> GetMyBookingsAsync(string username);
        Task<int> CreateEventAsync(EventDto dto);
        Task<EventDto?> GetEventByIdAsync(int id);
        Task UpdateEventAsync(EventDto dto);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<EventDto>> GetUpcomingEventsAsync(EventType? eventType = null);
        Task AddSeatsAsync(List<Seat> seats);
        Task CancelBookingAsync(int bookingId, string username);
        Task<IEnumerable<EventDto>> GetUpcomingEventsByType(EventType eventType);
    }
}
