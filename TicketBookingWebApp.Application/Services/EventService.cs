using AutoMapper;
using Microsoft.Extensions.Logging;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EventService> _logger;
    private readonly TicketBookingSystemContext _context; // Inject DbContext

    public EventService(
        IEventRepository eventRepository,
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<EventService> logger,
        TicketBookingSystemContext context)  
    {
        _eventRepository = eventRepository;
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _context = context;
    }

    public async Task<BookingDto> BookSeatsAsync(int eventId, List<int> seatIds, string username)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found.");

            var evt = await _eventRepository.GetEventByIdAsync(eventId)
                ?? throw new Exception("Event not found.");

            if (evt.EventDateTime < DateTime.UtcNow)
                throw new Exception("Cannot book seats for a past event.");

            if (evt.AvailableTickets < seatIds.Count)
                throw new Exception("Not enough available tickets.");

            var seatsToBook = await _eventRepository.GetSeatsByIdsAsync(seatIds);

            if (seatsToBook.Count != seatIds.Count)
                throw new Exception("Some selected seats are not available.");

            foreach (var seat in seatsToBook)
            {
                if (seat.IsBooked)
                    throw new Exception($"Seat {seat.SeatNumber} is already booked.");

                seat.IsBooked = true;
            }

            evt.AvailableTickets -= seatIds.Count;

            var booking = new Booking
            {
                EventId = eventId,
                UserId = user.Id,
                BookingDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                Quantity = seatIds.Count,
                Username = user.UserName,
                EventTitle = evt.Title,
                EventDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                IsSeatBased = evt.IsSeatBased,
                SeatIds = string.Join(",", seatIds)
            };

            _context.Bookings.Add(booking);
            _context.Seats.UpdateRange(seatsToBook);
            _context.Events.Update(evt);

            await _context.SaveChangesAsync(); 

            await transaction.CommitAsync();
            return _mapper.Map<BookingDto>(booking);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(); // Rollback on any error
            _logger.LogError(ex, "Failed to book seats");
            throw;
        }
    }


    public async Task<BookingDto> BookGeneralTicketsAsync(int eventId, int quantity, string username)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found.");

            var evt = await _eventRepository.GetEventByIdAsync(eventId)
                ?? throw new Exception("Event not found.");

            if (evt.EventDateTime < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")))
                throw new Exception("Cannot book tickets for a past event.");

            if (evt.AvailableTickets < quantity)
                throw new Exception("Not enough tickets available.");

            evt.AvailableTickets -= quantity;

            var booking = new Booking
            {
                EventId = eventId,
                UserId = user.Id,
                BookingDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                Quantity = quantity,
                EventDateTime = evt.EventDateTime,
                EventTitle = evt.Title,
                IsSeatBased = evt.IsSeatBased,
                Username = username
            };

            await _bookingRepository.AddAsync(booking);

            _context.Events.Update(evt);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return _mapper.Map<BookingDto>(booking);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error booking general tickets for user {User} on event {EventId}", username, eventId);
            throw;
        }
    }
    public async Task CancelBookingAsync(int bookingId, string username)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Get booking by ID
            var booking = await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new Exception("Booking not found.");

            // Verify whether its same users booking or not
            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found.");

            if (booking.UserId != user.Id)
                throw new UnauthorizedAccessException("You can only cancel your own bookings.");

            // This will check whether the vent has started or initiated or not
            var evt = await _eventRepository.GetEventByIdAsync(booking.EventId)
                ?? throw new Exception("Associated event not found.");

            if (evt.EventDateTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot cancel booking after the event has started.");

            // Here Th eupdated tickets will be shown
            evt.AvailableTickets += booking.Quantity;

            // If seat-based booking, free seats
            if (booking.IsSeatBased && !string.IsNullOrEmpty(booking.SeatIds))
            {
                var seatIds = booking.SeatIds.Split(',').Select(int.Parse).ToList();
                var seats = await _eventRepository.GetSeatsByIdsAsync(seatIds);

                foreach (var seat in seats)
                {
                    seat.IsBooked = false;
                }

                _context.Seats.UpdateRange(seats);
            }

            _context.Events.Update(evt);

            // Remove booking record
            _bookingRepository.Delete(booking);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
        if (booking == null)
            return null;

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username)
            ?? throw new Exception("User not found.");

        var bookings = await _bookingRepository.GetByUserIdAsync(user.Id);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<int> CreateEventAsync(EventDto dto)
    {
        if (dto.EventDateTime <= DateTime.UtcNow)
            throw new Exception("Event date must be in the future.");

        var evt = _mapper.Map<Event>(dto);
        evt.CreatedAt = DateTime.UtcNow;
        await _eventRepository.AddEventAsync(evt);
        return (evt.Id);
    }

    public async Task<EventDto?> GetEventByIdAsync(int id)
    {
        var evt = await _eventRepository.GetEventByIdAsync(id);
        return evt == null ? null : _mapper.Map<EventDto>(evt);
    }

    public async Task UpdateEventAsync(EventDto dto)
    {
        var existing = await _eventRepository.GetEventByIdAsync(dto.Id);
        if (existing == null)
            throw new Exception("Event not found.");

        if (dto.EventDateTime <= DateTime.UtcNow)
            throw new Exception("Event date must be in the future.");

        int ticketDifference = dto.TotalTickets - existing.TotalTickets;

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.EventDateTime = dto.EventDateTime;
        existing.VenueId = dto.VenueId;
        existing.Price = dto.Price;
        existing.TotalTickets = dto.TotalTickets;

        existing.IsSeatBased = dto.IsSeatBased;

        if (ticketDifference != 0)
        {
            existing.AvailableTickets += ticketDifference;
            if (existing.AvailableTickets < 0)
                throw new Exception("Cannot reduce total tickets below the number of already booked tickets.");
        }

        await _eventRepository.UpdateEventAsync(existing);
        if (dto.IsSeatBased && ticketDifference > 0)
        {
            var currentSeats = await _eventRepository.GetSeatsByEventIdAsync(dto.Id);
            var newSeats = new List<Seat>();

            for (int i = 1; i <= ticketDifference; i++)
            {
                newSeats.Add(new Seat
                {
                    SeatNumber = $"S{currentSeats.Count + i}",
                    IsAvailable = true,
                    IsBooked = false,
                    EventId = existing.Id
                });
            }

            await _eventRepository.AddSeatsAsync(newSeats);
            await _eventRepository.SaveChangesAsync();
        }
    }

    public async Task AddSeatsAsync(List<Seat> seats)
    {
        if (seats == null || seats.Count == 0)
            throw new ArgumentException("Seats list cannot be null or empty.");

        await _eventRepository.AddSeatsAsync(seats);
        await _eventRepository.SaveChangesAsync();
    }
    public async Task DeleteEventAsync(int id)
    {
        await _eventRepository.DeleteEventAsync(id);
    }
    public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
    {
        var events = await _eventRepository.GetAllEventsAsync();
        // Optionally filter upcoming events or return all
        return _mapper.Map<IEnumerable<EventDto>>(events);
    }

    public async Task<EventDto?> GetEventDetailsAsync(int eventId)
    {
        var evt = await _eventRepository.GetEventByIdAsync(eventId);
        if (evt == null)
            return null;

        var eventDto = _mapper.Map<EventDto>(evt);

        if (evt.IsSeatBased)
        {
            var seats = await _eventRepository.GetSeatsByEventIdAsync(eventId);
            eventDto.Seats = _mapper.Map<List<Seat>>(seats);
        }

        return eventDto;
    }

    public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync()
    {
        var events = await _eventRepository.GetUpcomingEventsAsync();
        return _mapper.Map<IEnumerable<EventDto>>(events);
    }

}
