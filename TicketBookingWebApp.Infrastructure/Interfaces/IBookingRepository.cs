using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Common.Pagination;
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
    Task<List<Seat>> GetAvailableSeatsWithRowVersionAsync(List<int> seatIds);
    Task UpdateSeatsAsync(List<Seat> seats);
    DbContext GetDbContext();
    Task<int> GetUserIdByUsernameAsync(string username);
    Task<Booking?> GetBookingByIdAsync(int bookingId);
    void Delete(Booking booking);

}
