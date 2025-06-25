using TicketBookingWebApp.Common.Pagination;
using TicketBookingWebApp.Domain.Entities;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<PagedResult<Event>> GetAllEventsPagedAsync(int pageNumber, int pageSize);
    Task<Event?> GetEventByIdAsync(int id);
    Task AddEventAsync(Event ev);
    Task UpdateEventAsync(Event ev);
    Task DeleteEventAsync(int id);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync();
    Task<IEnumerable<Event>> GetUpcomingEventsByTypeAsync(int type);
    Task<PagedResult<Event>> GetUpcomingEventsPagedAsync(int pageNumber, int pageSize, int? eventType, string? searchTerm);
    Task<PagedResult<Event>> GetUpcomingEventsByTypePagedAsync(int type, int pageNumber, int pageSize);
    Task AddSeatsAsync(List<Seat> seats);
    Task SaveChangesAsync();
    Task<List<Seat>> GetSeatsByEventIdAsync(int eventId);
    Task<List<Seat>> GetSeatsByIdsAsync(List<int> seatIds);
    Task UpdateSeatsAsync(List<Seat> seats);
    IQueryable<Event> GetEventsAsQueryable();
}
