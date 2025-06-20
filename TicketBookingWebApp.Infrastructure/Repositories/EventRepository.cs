using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Domain.Enums;

public class EventRepository : IEventRepository
{
    private readonly TicketBookingSystemContext _context;

    public EventRepository(TicketBookingSystemContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await _context.Events
            .Include(e => e.Venue)
            .ToListAsync();
    }
    public async Task AddSeatsAsync(List<Seat> seats)
    {
        _context.Seats.AddRangeAsync(seats);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _context.Events
    .Include(e => e.Venue)
    .FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task<List<Seat>> GetSeatsByEventIdAsync(int eventId)
    {
        return await _context.Seats
            .Where(s => s.EventId == eventId)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<List<Seat>> GetSeatsByIdsAsync(List<int> seatIds)
    {
        return await _context.Seats
            .Where(s => seatIds.Contains(s.Id))
            .ToListAsync();
    }

    public async Task UpdateSeatsAsync(List<Seat> seats)
    {
        _context.Seats.UpdateRange(seats);
        await _context.SaveChangesAsync();
    }

    public async Task AddEventAsync(Event ev)
    {
        _context.Events.Add(ev);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(Event ev)
    {
        _context.Events.Update(ev);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev != null)
        {
            if (ev.IsSeatBased == true)
            {
                var seats = await _context.Seats
                                          .Where(s => s.EventId == id)
                                          .ToListAsync();

                _context.Seats.RemoveRange(seats);
            }

            _context.Events.Remove(ev);

            await _context.SaveChangesAsync();
        }
    }
    public async Task<IEnumerable<Event>> GetUpcomingEventsByTypeAsync(int type)
    {
        return await _context.Events
            .Where(e => e.EventDateTime > DateTime.UtcNow && e.EventType == type)
            .ToListAsync();
    }


    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Events
            .Include(e => e.Venue)
            .Where(e => e.EventDateTime > now)
            .ToListAsync();
    }





}
