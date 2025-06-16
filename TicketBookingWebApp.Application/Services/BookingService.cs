using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Exceptions;
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
            var dbContext = _bookingRepository.GetDbContext();

            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var eventEntity = await _eventRepository.GetEventByIdAsync(bookingDto.EventId);
                if (eventEntity == null)
                    return false;

                var userId = await _bookingRepository.GetUserIdByUsernameAsync(bookingDto.Username);
                if (userId == 0)
                    throw new InvalidOperationException("User not found");

                var booking = new Booking
                {
                    EventId = bookingDto.EventId,
                    UserId = userId,
                    Username = bookingDto.Username,
                    EventTitle = eventEntity.Title,
                    EventDateTime = eventEntity.EventDateTime,
                    IsSeatBased = bookingDto.IsSeatBased,
                    Quantity = bookingDto.Quantity,
                    BookingDate = DateTime.UtcNow,
                    SeatIds = bookingDto.IsSeatBased
                        ? string.Join(",", bookingDto.SeatIds)
                        : null
                };

                if (bookingDto.IsSeatBased)
                {
                    var seats = await _bookingRepository.GetAvailableSeatsWithRowVersionAsync(bookingDto.SeatIds);
                    if (seats.Count != bookingDto.SeatIds.Count)
                        throw new ConcurrencyException("Some selected seats are no longer available.");

                    foreach (var seat in seats)
                    {
                        seat.IsBooked = true;
                    }

                    await _bookingRepository.UpdateSeatsAsync(seats);
                }

                await _bookingRepository.AddAsync(booking);
                await transaction.CommitAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new ConcurrencyException("Booking failed due to concurrent seat updates.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
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
