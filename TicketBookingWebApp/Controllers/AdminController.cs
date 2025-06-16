using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IVenueService _venueService;

        public AdminController(IEventService eventService, IVenueService venueService)
        {
            _eventService = eventService;
            _venueService = venueService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllEventsAsync();
            return View(events);
        }

        public async Task<IActionResult> CreateEvent()
        {
            var venues = await _venueService.GetAllVenuesAsync();
            List<SelectListItem> selectListItems = venues.Select(item => new SelectListItem() { Text = item.Name, Value = item.Id.ToString() }).ToList();
            ViewBag.Venues = selectListItems;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventDto dto)
        {
            if (!ModelState.IsValid)
            {
                var venues = await _venueService.GetAllVenuesAsync();
                List<SelectListItem> selectListItems = venues.Select(item => new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }).ToList();
                ViewBag.Venues = selectListItems;
                dto.AvailableTickets = dto.TotalTickets;
                var eventId = await _eventService.CreateEventAsync(dto);

                if (dto.IsSeatBased && dto.TotalTickets > 0)
                {
                    var seats = new List<Seat>();
                    for (int i = 1; i <= dto.TotalTickets; i++)
                    {
                        seats.Add(new Seat
                        {
                            SeatNumber = $"S{i}",
                            IsAvailable = true,
                            IsBooked = false,
                            EventId = eventId,
                            //RowVersion = new byte[8] // Placeholder; your DB may auto-handle this
                        });
                    }

                    await _eventService.AddSeatsAsync(seats);
                    return RedirectToAction("Index",dto);
                }
 
            }
            return RedirectToAction("Index");
            
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            var venues = await _venueService.GetAllVenuesAsync();
            List<SelectListItem> selectListItems = venues.Select(item => new SelectListItem() { Text = item.Name, Value = item.Id.ToString() }).ToList();
            ViewBag.Venues = selectListItems;
            return View(eventDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(EventDto dto)
        {
            if (!ModelState.IsValid)
            {
                var venues = await _venueService.GetAllVenuesAsync();
                List<SelectListItem> selectListItems = venues.Select(item => new SelectListItem() { Text = item.Name, Value = item.Id.ToString() }).ToList();
                ViewBag.Venues = selectListItems;
                return View(dto);
            }

            await _eventService.UpdateEventAsync(dto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ManageVenues()
        {
            var venues = await _venueService.GetAllVenuesAsync();
            return View(venues);
        }
        [HttpGet]
        public async Task<IActionResult> AddVenue() => View();

        [HttpPost]
        public async Task<IActionResult> AddVenue(VenueDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _venueService.CreateVenueAsync(dto);
            return RedirectToAction("Index");
        }
    }
}