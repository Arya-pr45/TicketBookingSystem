using TicketBookingWebApp.Domain.Entities;

public interface IBookingRepository
{
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<IEnumerable<Booking>> GetBookingsByUsernameAsync(string username);
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
    Task<Booking?> GetByIdAsync(int bookingId);
    Task DeleteAsync(int bookingId);
    Task AddAsync(Booking booking);
}
