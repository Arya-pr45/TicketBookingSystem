using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Application.Services;
using TicketBookingWebApp.Domain.Enums;
using TicketBookingWebApp.Common.Pagination;
using TicketBookingWebApp.Application.DTOs.Filters;

namespace TicketBookingWebApp.Application.Interfaces
{
    public interface IEventService
    {
        Task<PagedResult<EventDto>> GetAllEventsAsync(int pageNumber = 1, int pageSize = 10);

        Task<BookingDto> BookSeatsAsync(int eventId, List<int> seatIds, User user);
        Task<BookingDto> BookGeneralTicketsAsync(int eventId, int quantity, User user);
        Task<BookingDto?> GetBookingByIdAsync(int bookingId);

        Task<IEnumerable<BookingDto>> GetMyBookingsAsync(string username);
        Task<int> CreateEventAsync(EventDto dto);
        Task<EventDto?> GetEventByIdAsync(int id);
        Task UpdateEventAsync(EventDto dto);
        Task DeleteEventAsync(int id);
        Task<PagedResult<EventDto>> GetUpcomingEventsAsync(EventType? eventType = null, int pageNumber = 1, int pageSize = 10);
        Task AddSeatsAsync(List<Seat> seats);
        Task CancelBookingAsync(int bookingId, string username);
        Task<PagedResult<EventDto>> GetUpcomingEventsByTypeAsync(EventType eventType, int pageNumber = 1, int pageSize = 10);
        Task<EventDto?> GetEventDetailsAsync(int eventId);
        Task<PagedResult<EventDto>> GetFilteredEventsAsync(EventFilterParams filters, int pageNumber, int pageSize);
    }
}
