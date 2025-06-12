using System;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Application.Interfaces;

namespace TicketBookingWebApp.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IVenueRepository _venueRepo;

        public AdminService(IEventRepository eventRepo, IVenueRepository venueRepo)
        {
            _eventRepo = eventRepo;
            _venueRepo = venueRepo;
        }

        // Events
        public async Task<IEnumerable<Event>> GetAllEventsAsync() => await _eventRepo.GetAllEventsAsync();
        public async Task<Event?> GetEventByIdAsync(int id) => await _eventRepo.GetEventByIdAsync(id);
        public async Task CreateEventAsync(Event ev) => await _eventRepo.AddEventAsync(ev);
        public async Task UpdateEventAsync(Event ev) => await _eventRepo.UpdateEventAsync(ev);
        public async Task DeleteEventAsync(int id) => await _eventRepo.DeleteEventAsync(id);

        // Venues
        public async Task<IEnumerable<Venue>> GetAllVenuesAsync() => await _venueRepo.GetAllVenuesAsync();
        public async Task<Venue?> GetVenueByIdAsync(int id) => await _venueRepo.GetVenueByIdAsync(id);
        public async Task CreateVenueAsync(Venue venue) => await _venueRepo.AddVenueAsync(venue);
        public async Task UpdateVenueAsync(Venue venue) => await _venueRepo.UpdateVenueAsync(venue);
        public async Task DeleteVenueAsync(int id) => await _venueRepo.DeleteVenueAsync(id);
    }
}
