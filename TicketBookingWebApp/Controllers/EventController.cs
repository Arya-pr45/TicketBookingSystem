using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.Services;

namespace TicketBookingWebApp.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var events = await _eventService.GetUpcomingEventsAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                events = events
                    .Where(e => e.Title.ToLower().Contains(searchTerm))
                    .ToList();
            }

            ViewBag.CurrentFilter = searchTerm;
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
                return BadRequest("Invalid booking data.");
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            try
            {
                BookingDto booking;

                if (model.IsSeatBased)
                {
                    booking = await _eventService.BookSeatsAsync(model.EventId, model.SeatIds, username);
                }
                else
                {
                    booking = await _eventService.BookGeneralTicketsAsync(model.EventId, model.Quantity, username);
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

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            try
            {
                await _eventService.CancelBookingAsync(bookingId, username);
                return Ok(new { message = "Booking cancelled successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
