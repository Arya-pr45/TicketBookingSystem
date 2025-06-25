using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.DTOs.Filters;
using TicketBookingWebApp.Application.Exceptions;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Common.Pagination;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Domain.Enums;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EventService> _logger;
    private readonly IEmailService _emailService;
    private readonly TicketBookingSystemContext _context;

    public EventService(
        IEventRepository eventRepository,
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<EventService> logger,
        TicketBookingSystemContext context,
        IEmailService emailService)
    {
        _eventRepository = eventRepository;
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _context = context;
        _emailService = emailService;
    }
    public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync(EventType? eventType = null)
    {
        IEnumerable<Event> events;

        if (eventType.HasValue)
        {
            events = await _eventRepository.GetUpcomingEventsByTypeAsync((int)eventType.Value);
        }
        else
        {
            events = await _eventRepository.GetUpcomingEventsAsync();
        }

        return _mapper.Map<IEnumerable<EventDto>>(events);
    }
    public async Task<PagedResult<EventDto>> GetUpcomingEventsByTypeAsync(EventType eventType, int pageNumber = 1, int pageSize = 10)
    {
        var query = _eventRepository.GetEventsAsQueryable()
                                    .Where(e => e.EventDateTime > DateTime.UtcNow && e.EventType == (int)eventType);

        var totalCount = await query.CountAsync();

        var events = await query
            .OrderBy(e => e.EventDateTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mapped = _mapper.Map<IEnumerable<EventDto>>(events);

        return new PagedResult<EventDto>
        {
            Items = mapped,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }



    public async Task<BookingDto> BookSeatsAsync(int eventId, List<int> seatIds, User user)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var userName = await _userRepository.GetByUsernameAsync(user.UserName)
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

            var referenceNumber = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var booking = new Booking
            {
                EventId = eventId,
                UserId = user.Id,
                ReferenceNumber = referenceNumber,
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

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency conflict while booking seats for event {EventId}", eventId);
                throw new InvalidOperationException("The seats or event were modified by another user. Please try again.");
            }

            var bookingDto = _mapper.Map<BookingDto>(booking);
            bookingDto.Email = user.Email;
            await _emailService.SendBookingConfirmationEmailAsync(bookingDto);
            return bookingDto;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to book seats");
            throw;
        }
    }



    public async Task<BookingDto> BookGeneralTicketsAsync(int eventId, int quantity, User user)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var userName = await _userRepository.GetByUsernameAsync(user.UserName)
                ?? throw new Exception("User not found.");

            var evt = await _eventRepository.GetEventByIdAsync(eventId)
                ?? throw new Exception("Event not found.");

            if (evt.EventDateTime < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")))
                throw new Exception("Cannot book tickets for a past event.");

            if (evt.AvailableTickets < quantity)
                throw new Exception("Not enough tickets available.");

            evt.AvailableTickets -= quantity;

            var referenceNumber = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var booking = new Booking
            {
                EventId = eventId,
                ReferenceNumber = referenceNumber,
                UserId = userName.Id,
                BookingDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                Quantity = quantity,
                EventDateTime = evt.EventDateTime,
                EventTitle = evt.Title,
                IsSeatBased = evt.IsSeatBased,
                Username = user.UserName
            };

            await _bookingRepository.AddAsync(booking);
            _context.Events.Update(evt);

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency conflict while booking general tickets for event {EventId}", eventId);
                throw new InvalidOperationException("The event was modified by another user. Please try again.");
            }

            var bookingDto = _mapper.Map<BookingDto>(booking);
            bookingDto.Email = user.Email;
            await _emailService.SendBookingConfirmationEmailAsync(bookingDto);
            return bookingDto;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error booking general tickets");
            throw;
        }
    }

    public async Task CancelBookingAsync(int bookingId, string username)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new BookingConflictException("Booking not found.");

            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found.");

            if (booking.UserId != user.Id)
                throw new UnauthorizedAccessException("You can only cancel your own bookings.");

            var evt = await _eventRepository.GetEventByIdAsync(booking.EventId)
                ?? throw new Exception("Associated event not found.");

            if (evt.EventDateTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot cancel booking after the event has started.");

            evt.AvailableTickets += booking.Quantity;

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
            _bookingRepository.Delete(booking);

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency conflict while cancelling booking {BookingId}", bookingId);
                throw new InvalidOperationException("The event or seats were modified by another user. Please try again.");
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to cancel booking");
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
        existing.EventType = dto.EventType;
        existing.ImageUrl = dto.ImageUrl;

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
        var currentSeats = await _eventRepository.GetSeatsByEventIdAsync(id);

        _context.Seats.RemoveRange(currentSeats);
        await _eventRepository.SaveChangesAsync();

        await _eventRepository.DeleteEventAsync(id);
    }
    public async Task<PagedResult<EventDto>> GetAllEventsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _eventRepository.GetEventsAsQueryable();

        var totalCount = await query.CountAsync();

        var events = await query
            .OrderBy(e => e.EventDateTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mapped = _mapper.Map<IEnumerable<EventDto>>(events);

        return new PagedResult<EventDto>
        {
            Items = mapped,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
    public async Task<PagedResult<EventDto>> GetFilteredEventsAsync(EventFilterParams filters, int pageNumber, int pageSize)
    {
        var query = _eventRepository.GetEventsAsQueryable();

        if (!string.IsNullOrEmpty(filters.Keyword))
            query = query.Where(e => e.Title.Contains(filters.Keyword));

        if (!string.IsNullOrEmpty(filters.EventType))
        {
            if (int.TryParse(filters.EventType, out var eventTypeValue))
            {
                query = query.Where(e => e.EventType == eventTypeValue);
            }
        }

        if (filters.FromDate.HasValue)
            query = query.Where(e => e.EventDateTime >= filters.FromDate.Value);

        if (filters.ToDate.HasValue)
            query = query.Where(e => e.EventDateTime <= filters.ToDate.Value);

        if (filters.MinPrice.HasValue)
            query = query.Where(e => e.Price >= filters.MinPrice.Value);

        if (filters.MaxPrice.HasValue)
            query = query.Where(e => e.Price <= filters.MaxPrice.Value);

        if (filters.OnlyAvailable == true)
            query = query.Where(e => e.AvailableTickets > 0);

        query = filters.SortBy switch
        {
            "price" => query.OrderBy(e => e.Price),
            "date" => query.OrderBy(e => e.EventDateTime),
            _ => query.OrderByDescending(e => e.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var events = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mapped = _mapper.Map<IEnumerable<EventDto>>(events);

        return new PagedResult<EventDto>
        {
            Items = mapped,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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

    public async Task<PagedResult<EventDto>> GetUpcomingEventsAsync(EventType? eventType = null, int pageNumber = 1, int pageSize = 10)
    {
        IQueryable<Event> query = _eventRepository.GetEventsAsQueryable()
                                                  .Where(e => e.EventDateTime > DateTime.UtcNow);

        if (eventType.HasValue)
        {
            query = query.Where(e => e.EventType == (int)eventType.Value);
        }

        var totalCount = await query.CountAsync();

        var events = await query
            .OrderBy(e => e.EventDateTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mapped = _mapper.Map<IEnumerable<EventDto>>(events);

        return new PagedResult<EventDto>
        {
            Items = mapped,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }


}