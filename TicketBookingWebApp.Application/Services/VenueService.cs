using AutoMapper;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Application.Interfaces;

namespace TicketBookingWebApp.Application.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;

        public VenueService(IVenueRepository venueRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VenueDto>> GetAllVenuesAsync()
        {
            var venues = await _venueRepository.GetAllVenuesAsync();
            return _mapper.Map<IEnumerable<VenueDto>>(venues);
        }

        public async Task<VenueDto?> GetVenueByIdAsync(int id)
        {
            var venue = await _venueRepository.GetVenueByIdAsync(id);
            return venue == null ? null : _mapper.Map<VenueDto>(venue);
        }

        public async Task<bool> CreateVenueAsync(VenueDto venueDto)
        {
            var venue = _mapper.Map<Venue>(venueDto);
            await _venueRepository.AddVenueAsync(venue);
            return true;
        }

        public async Task<bool> UpdateVenueAsync(VenueDto venueDto)
        {
            var venue = _mapper.Map<Venue>(venueDto);
            await _venueRepository.UpdateVenueAsync(venue);
            return true;
        }

        public async Task<bool> DeleteVenueAsync(int id)
        {
            await _venueRepository.DeleteVenueAsync(id);
            return true;
        }

        public async Task<IEnumerable<Venue>> GetAllAsync()
        {
            return await _venueRepository.GetAllVenuesAsync();
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _venueRepository.GetVenueByIdAsync(id);
        }

        public async Task AddAsync(Venue venue)
        {
            await _venueRepository.AddVenueAsync(venue);
        }

        public async Task UpdateAsync(Venue venue)
        {
            await _venueRepository.UpdateVenueAsync(venue);
        }

        public async Task DeleteAsync(int id)
        {
            await _venueRepository.DeleteVenueAsync(id);
        }
    }
}
