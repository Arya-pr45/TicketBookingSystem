using AutoMapper;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepository,
            IEventRepository eventRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            return booking == null ? null : _mapper.Map<BookingDto>(booking);
        }

        public async Task<bool> CreateBookingAsync(BookingDto bookingDto)
        {
            try
            {
                var booking = _mapper.Map<Booking>(bookingDto);
                booking.BookingDate = DateTime.UtcNow;

                // Optionally validate event exists
                var eventExists = await _eventRepository.GetEventByIdAsync(booking.EventId);
                if (eventExists == null) return false;

                await _bookingRepository.AddAsync(booking);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                await _bookingRepository.DeleteAsync(bookingId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
