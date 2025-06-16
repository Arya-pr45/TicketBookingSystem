using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Infrastructure.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        private readonly TicketBookingSystemContext _context;

        public VenueRepository(TicketBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Venue>> GetAllVenuesAsync()
        {
            return await _context.Venues.ToListAsync();
        }

        public async Task<Venue?> GetVenueByIdAsync(int id)
        {
            return await _context.Venues.FindAsync(id);
        }

        public async Task AddVenueAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVenueAsync(Venue venue)
        {
            _context.Venues.Update(venue);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVenueAsync(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
        }
    }
}