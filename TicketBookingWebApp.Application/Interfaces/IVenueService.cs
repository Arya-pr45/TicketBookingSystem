using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Application.Interfaces
{
    public interface IVenueService
    {
        //Task<IEnumerable<BookingDto>> GetBookingsByUsernameAsync(string username);
        Task<IEnumerable<VenueDto>> GetAllVenuesAsync();
        Task<VenueDto?> GetVenueByIdAsync(int id);
        Task<bool> CreateVenueAsync(VenueDto venueDto);
        Task<bool> UpdateVenueAsync(VenueDto venueDto);
        Task<bool> DeleteVenueAsync(int id);

        Task<IEnumerable<Venue>> GetAllAsync();
        Task<Venue?> GetByIdAsync(int id);
        Task AddAsync(Venue venue);
        Task UpdateAsync(Venue venue);
        Task DeleteAsync(int id);
    }
}
