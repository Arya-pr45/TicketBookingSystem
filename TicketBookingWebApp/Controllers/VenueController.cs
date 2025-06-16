using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VenueController : Controller
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        public async Task<IActionResult> Index()
        {
            var venues = await _venueService.GetAllAsync();
            return View(venues);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VenueDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var venue = new Venue
            {
                Id = dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                Capacity = dto.Capacity
            };

            await _venueService.AddAsync(venue);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VenueDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var venue = new Venue
            {
                Id = dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                Capacity = dto.Capacity
            };

            await _venueService.UpdateAsync(venue);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _venueService.GetByIdAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _venueService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
