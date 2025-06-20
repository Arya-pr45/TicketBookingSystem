using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Domain.Entities;

public class BookingRepository : IBookingRepository
{
    private readonly TicketBookingSystemContext _context;
    private readonly DbSet<Seat> _seats;
    private readonly DbSet<User> _users;

    public BookingRepository(TicketBookingSystemContext context)
    {
        _context = context;
        _seats = context.Set<Seat>();
        _users = context.Set<User>();
    }

    public DbContext GetDbContext() => _context;

    public async Task<int> GetUserIdByUsernameAsync(string username)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == username);

        return user?.Id ?? 0; // Assuming 0 means "not found"
    }


    public async Task<List<Seat>> GetAvailableSeatsWithRowVersionAsync(List<int> seatIds)
    {
        return await _seats
            .Where(s => seatIds.Contains(s.Id) && !s.IsBooked)
            .AsTracking()
            .ToListAsync();
    }

    public async Task UpdateSeatsAsync(List<Seat> seats)
    {
        _seats.UpdateRange(seats);
        await _context.SaveChangesAsync();
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
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
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
    public void Delete(Booking booking)
    {
        _context.Bookings.Remove(booking);
    }

    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        return await _context.Bookings
                             .Include(b => b.Event)
                             .Include(b => b.User)
                             .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

}