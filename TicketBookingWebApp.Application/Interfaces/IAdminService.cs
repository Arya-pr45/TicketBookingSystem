using System;
using TicketBookingWebApp.Domain.Entities;

public interface IAdminService
{
    // Events
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int id);
    Task CreateEventAsync(Event ev);
    Task UpdateEventAsync(Event ev);
    Task DeleteEventAsync(int id);
    Task<User?> GetUserProfileAsync(string userName);


    // Venues
    Task<IEnumerable<Venue>> GetAllVenuesAsync();
    Task<Venue?> GetVenueByIdAsync(int id);
    Task CreateVenueAsync(Venue venue);
    Task UpdateVenueAsync(Venue venue);
    Task DeleteVenueAsync(int id);
}
