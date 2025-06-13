using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.Services;

namespace TicketBookingWebApp.Web.Controllers
{
    //[Authorize(Roles = "User")]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var events = await _eventService.GetAllEventsAsync();

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

        public async Task<IActionResult> BookTicket(int id)
        {
            var eventDto = await _eventService.GetEventDetailsAsync(id);
            if (eventDto == null) return NotFound();

            var model = new EventDto
            {
                Id = eventDto.Id,
                Title = eventDto.Title,
                IsSeatBased = eventDto.IsSeatBased,
                AvailableSeatIds = eventDto.AvailableSeatIds,
                EventDateTime = eventDto.EventDateTime
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookTicket(BookingDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            BookingDto booking;

            try
            {
                var eventDetails = await _eventService.GetEventDetailsAsync(model.EventId);
                if (eventDetails == null) return NotFound();

   
                if (eventDetails.IsSeatBased)
                {
                    booking = await _eventService.BookSeatsAsync(model.EventId, eventDetails.AvailableSeatIds ?? new List<int>(), username);
                }
                else
                {
                    booking = await _eventService.BookGeneralTicketsAsync(model.EventId, model.Quantity, username);
                }

                return RedirectToAction("Confirmation", booking);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public IActionResult Confirmation(BookingDto booking)
        {
            return View(booking);
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
