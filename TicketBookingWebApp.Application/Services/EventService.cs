using AutoMapper;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public EventService(
            IEventRepository eventRepository,
            IBookingRepository bookingRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _eventRepository = eventRepository;
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto?> GetEventDetailsAsync(int eventId)
        {
            var evt = await _eventRepository.GetEventByIdAsync(eventId);
            return evt == null ? null : _mapper.Map<EventDto>(evt);
        }

        public async Task<BookingDto> BookSeatsAsync(int eventId, List<int> seatIds, string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found.");

            var booking = new Booking
            {
                EventId = eventId,
                UserId = user.Id,
                BookingDate = DateTime.UtcNow,
                BookingDetails = seatIds.Select(seatId => new BookingDetail
                {
                    SeatId = seatId,
                    Quantity = 1
                }).ToList()
            };

            await _bookingRepository.AddAsync(booking);
            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<BookingDto> BookGeneralTicketsAsync(int eventId, int quantity, string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found.");

            var booking = new Booking
            {
                EventId = eventId,
                UserId = user.Id,
                BookingDate = DateTime.UtcNow,
                BookingDetails = new List<BookingDetail>
                {
                    new BookingDetail
                    {
                        Quantity = quantity
                    }
                }
            };

            await _bookingRepository.AddAsync(booking);
            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found.");

            var bookings = await _bookingRepository.GetByUserIdAsync(user.Id);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        public async Task CreateEventAsync(EventDto dto)
        {
            var evt = _mapper.Map<Event>(dto);
            await _eventRepository.AddEventAsync(evt);
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            var evt = await _eventRepository.GetEventByIdAsync(id);
            return evt == null ? null : _mapper.Map<EventDto>(evt);
        }

        public async Task UpdateEventAsync(EventDto dto)
        {
            var existing = await _eventRepository.GetEventByIdAsync(dto.Id);
            if (existing == null) throw new Exception("Event not found.");

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.EventDateTime = dto.EventDateTime;
            existing.VenueId = dto.VenueId;
            existing.TotalTickets = dto.TotalTickets;
            //existing.Price = dto.Price;

            await _eventRepository.UpdateEventAsync(existing);
        }

        public async Task DeleteEventAsync(int id)
        {
            await _eventRepository.DeleteEventAsync(id);
        }

    }
}
