using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.Services;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Domain.Enums;

namespace TicketBookingWebApp.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IBookingService _bookingService;

        public EventController(IEventService eventService, IBookingService bookingService)
        {
            _eventService = eventService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(string? searchTerm, int? eventType)
        {
            EventType? eventTypeEnum = eventType.HasValue ? (EventType?)eventType.Value : null;

            var events = await _eventService.GetUpcomingEventsAsync((EventType?)eventType);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                events = events
                    .Where(e => e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.CurrentFilter = searchTerm;
            ViewBag.SelectedType = eventType;
            ViewBag.EventTypes = Enum.GetValues(typeof(EventType)).Cast<EventType>();

            return View(events);
        }
        public async Task<IActionResult> Details(int id)
        {
            var eventDto = await _eventService.GetEventDetailsAsync(id);
            if (eventDto == null) return NotFound();
            return View(eventDto);
        }

        [HttpPost]
        public async Task<IActionResult> BookTicket([FromBody] BookingDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                return BadRequest(new { message = "Invalid booking data", errors });
            }
            var user = new User()
            {
                UserName = User.FindFirst(ClaimTypes.Name)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value
            };
            if (string.IsNullOrEmpty(user.UserName))
                return Unauthorized();

            try
            {
                BookingDto booking;

                if (model.IsSeatBased)
                {
                    if (model.SeatIds == null || !model.SeatIds.Any())
                        return BadRequest("Seat selection is required for seat-based booking.");

                    booking = await _eventService.BookSeatsAsync(model.EventId, model.SeatIds, user);
                }
                else
                {
                    booking = await _eventService.BookGeneralTicketsAsync(model.EventId, model.Quantity, user);
                }

                return Ok(new { bookingId = booking.BookingId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> BookingDetails(int bookingId)
        {
            var booking = await _eventService.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            try
            {
                await _eventService.CancelBookingAsync(bookingId, username);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return RedirectToAction("MyBookings");
        }

        public async Task<IActionResult> MyBookings()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var bookings = await _eventService.GetMyBookingsAsync(username);
            return View(bookings);
        }
    }
}
