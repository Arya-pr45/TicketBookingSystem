using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Domain.Entities;

public class BookingRepository : IBookingRepository
{
    private readonly TicketBookingWebAppContext _context;

    public BookingRepository(TicketBookingWebAppContext context)
    {
        _context = context;
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUsernameAsync(string username)
    {
        return await _context.Bookings
            .Include(b => b.Event)
            .Where(b => b.User.UserName == username)
            .ToListAsync();
    }
    public async Task AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
    {
        return await _context.Bookings
            .Include(b => b.BookingDetails)
            .Include(b => b.Event)
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }
    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
    {
        return await _context.Bookings
            .Include(b => b.Event)
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int bookingId)
    {
        return await _context.Bookings
            .Include(b => b.Event)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }
    public async Task DeleteAsync(int bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}