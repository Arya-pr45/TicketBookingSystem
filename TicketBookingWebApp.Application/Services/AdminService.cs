using System;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Common.Pagination;
using TicketBookingWebApp.Infrastructure.Interfaces;

namespace TicketBookingWebApp.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IVenueRepository _venueRepo;
        private readonly IAdminRepository _adminRepo;


        public AdminService(IEventRepository eventRepo, IVenueRepository venueRepo, IAdminRepository adminRepo)
        {
            _eventRepo = eventRepo;
            _venueRepo = venueRepo;
            _adminRepo = adminRepo;
        }

        // Events
        public async Task<IEnumerable<Event>> GetAllEventsAsync() => await _eventRepo.GetAllEventsAsync();
        public async Task<Event?> GetEventByIdAsync(int id) => await _eventRepo.GetEventByIdAsync(id);
        public async Task CreateEventAsync(Event ev) => await _eventRepo.AddEventAsync(ev);
        public async Task UpdateEventAsync(Event ev) => await _eventRepo.UpdateEventAsync(ev);
        public async Task DeleteEventAsync(int id) => await _eventRepo.DeleteEventAsync(id);
        public async Task<PagedResult<Event>> GetPagedEventsAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _eventRepo.GetEventsAsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e => e.Title.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var events = await query
                .OrderBy(e => e.EventDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Event>
            {
                Items = events,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<User?> GetUserProfileAsync(string userName)
        {
            return await _adminRepo.GetUserByUserNameAsync(userName);
        }



        // Venues
        public async Task<IEnumerable<Venue>> GetAllVenuesAsync() => await _venueRepo.GetAllVenuesAsync();
        public async Task<Venue?> GetVenueByIdAsync(int id) => await _venueRepo.GetVenueByIdAsync(id);
        public async Task CreateVenueAsync(Venue venue) => await _venueRepo.AddVenueAsync(venue);
        public async Task UpdateVenueAsync(Venue venue) => await _venueRepo.UpdateVenueAsync(venue);
        public async Task DeleteVenueAsync(int id) => await _venueRepo.DeleteVenueAsync(id);
    }
}
