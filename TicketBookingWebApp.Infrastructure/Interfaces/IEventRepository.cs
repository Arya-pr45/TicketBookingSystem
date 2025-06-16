using TicketBookingWebApp.Domain.Entities;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int id);
    Task AddEventAsync(Event ev);
    Task UpdateEventAsync(Event ev);
    Task DeleteEventAsync(int id);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync();
    Task AddSeatsAsync(List<Seat> seats);

    Task SaveChangesAsync();
    Task<List<Seat>> GetSeatsByEventIdAsync(int eventId);
    Task<List<Seat>> GetSeatsByIdsAsync(List<int> seatIds);
    Task UpdateSeatsAsync(List<Seat> seats);
}
