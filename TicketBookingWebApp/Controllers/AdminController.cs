using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TicketBookingWebApp.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IVenueService _venueService;

        public AdminController(IEventService eventService, IVenueService venueService)
        {
            _eventService = eventService;
            _venueService = venueService;
        }

        public IActionResult Index()
        {
            return View();
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
                List<SelectListItem> selectListItems = venues.Select(item => new SelectListItem() { Text = item.Name, Value = item.Id.ToString() }).ToList();
                ViewBag.Venues = selectListItems;
                return View(dto);
            }

            await _eventService.CreateEventAsync(dto);
            return RedirectToAction("Index");
        }

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
