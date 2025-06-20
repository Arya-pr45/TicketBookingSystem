using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Services;

namespace TicketBookingWebApp.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class BookingController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IBookingService _bookingService;

        public BookingController(IEventService eventService, IBookingService bookingService)
        {
            _eventService = eventService;
            _bookingService = bookingService;
        }
        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllEventsAsync();
            return View(events);
        }

        public async Task<IActionResult> BookEvent(int eventId)
        {
            var eventDetails = await _eventService.GetEventByIdAsync(eventId);
            return View(new BookingDto { EventId = eventDetails.Id });
        }

        [HttpPost]
        public async Task<IActionResult> BookEvent(BookingDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.Name);
            dto.Username = userId;

            await _bookingService.CreateBookingAsync(dto);
            return RedirectToAction("MyBookings");
        }

        public async Task<IActionResult> CancelBooking(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return RedirectToAction("MyBookings");
        }
    }
}
