using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Domain.Entities;

public class EventRepository : IEventRepository
{
    private readonly TicketBookingWebAppContext _context;

    public EventRepository(TicketBookingWebAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Event?>> GetAllEventsAsync()
        => await _context.Events.ToListAsync();

    public async Task<Event?> GetEventByIdAsync(int id)
        => await _context.Events.FindAsync(id);

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
            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
        }
    }
}
