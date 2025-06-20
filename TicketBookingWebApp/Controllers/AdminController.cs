using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketBookingWebApp.Domain.Entities;
using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Humanizer;
using Mono.TextTemplating;
using Microsoft.AspNetCore.Components.Web;
using TicketBookingWebApp.Domain.Enums;

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
            ViewBag.Venues = venues.Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            }).ToList();

            ViewBag.EventTypes = Enum.GetValues(typeof(EventType))
                .Cast<EventType>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString(),
                }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var venues = await _venueService.GetAllVenuesAsync();
                    ViewBag.Venues = venues.Select(v => new SelectListItem
                    {
                        Text = v.Name,
                        Value = v.Id.ToString()
                    }).ToList();

                    ViewBag.EventTypes = Enum.GetValues(typeof(EventType))
                        .Cast<EventType>()
                        .Select(e => new SelectListItem
                        {
                            Value = ((int)e).ToString(),
                            Text = e.ToString(),
                            Selected = e == dto.Type
                        }).ToList();

                    return View(dto);
                }

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
                            EventId = eventId
                        });
                    }

                    await _eventService.AddSeatsAsync(seats);
                }

                TempData["Success"] = "Event created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while creating the event: {ex.Message}");
                return View(dto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);

            var venues = await _venueService.GetAllVenuesAsync();
            ViewBag.Venues = venues.Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            }).ToList();

            ViewBag.EventTypes = Enum.GetValues(typeof(EventType))
                .Cast<EventType>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString(),
                    Selected = e == eventDto.Type
                }).ToList();

            return View(eventDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(EventDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var venues = await _venueService.GetAllVenuesAsync();
                    ViewBag.Venues = venues.Select(v => new SelectListItem
                    {
                        Text = v.Name,
                        Value = v.Id.ToString()
                    }).ToList();

                    ViewBag.EventTypes = Enum.GetValues(typeof(EventType))
                        .Cast<EventType>()
                        .Select(e => new SelectListItem
                        {
                            Value = ((int)e).ToString(),
                            Text = e.ToString(),
                            Selected = dto.Type == e
                        }).ToList();

                    return View(dto);
                }
                await _eventService.UpdateEventAsync(dto);

                TempData["Success"] = "Event updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var venues = await _venueService.GetAllVenuesAsync();
                ViewBag.Venues = venues.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                }).ToList();

                ViewBag.EventTypes = Enum.GetValues(typeof(EventType))
                    .Cast<EventType>()
                    .Select(e => new SelectListItem
                    {
                        Value = ((int)e).ToString(),
                        Text = e.ToString(),
                        Selected = dto.Type == e
                    }).ToList();

                ModelState.AddModelError("", $"Update failed: {ex.Message}");
                return View(dto);
            }
        }


        private async Task SendEventUpdateConfirmationEmail(EventDto eventDto)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("TicketBookingSystem", "arya.mishra@coditas.com"));
            var recipient = "admin@yourapp.com"; // Change to actual admin email
            email.To.Add(new MailboxAddress("Admin", recipient));

            email.Subject = "Event Updated Successfully";

            var body = new TextPart("plain")
            {
                Text = $"Dear Admin,\n\n" +
                       $"The event '{eventDto.Title}' has been updated successfully.\n\n" +
                       $"Details:\n" +
                       $"Title: {eventDto.Title}\n" +
                       $"Date and Time: {eventDto.EventDateTime}\n" +
                       $"Venue: {eventDto.Venue.Name}\n" +
                       $"Tickets: {eventDto.TotalTickets}\n\n" +
                       "Best regards,\n" +
                       "Your App Team"
            };

            email.Body = body;

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync("smtp.yourserver.com", 587, SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync("your-email@domain.com", "your-email-password");

                    await client.SendAsync(email);

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Email sending failed: " + ex.Message, ex);
            }
        }


        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                TempData["Success"] = "Event deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting event: {ex.Message}";
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> ManageVenues(int? editId = null)
        {
            var venues = await _venueService.GetAllVenuesAsync();

            VenueDto venueToEdit = null;
            if (editId.HasValue)
            {
                var venue = await _venueService.GetByIdAsync(editId.Value);
                if (venue != null)
                {
                    venueToEdit = new VenueDto
                    {
                        Id = venue.Id,
                        Name = venue.Name,
                        Address = venue.Address,
                        Capacity = venue.Capacity
                    };
                }
            }

            ViewBag.Venues = venues;
            ViewBag.VenueToEdit = venueToEdit;

            return View(new VenueDto());
        }

        [HttpPost]
        public async Task<IActionResult> ManageVenues(VenueDto dto, string actionType)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Venues = await _venueService.GetAllVenuesAsync();
                ViewBag.VenueToEdit = dto;
                return View(dto);
            }

            if (actionType == "add")
            {
                await _venueService.CreateVenueAsync(dto);
                TempData["Success"] = "Venue added.";
            }
            else if (actionType == "edit")
            {
                var venue = new Venue
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Address = dto.Address,
                    Capacity = dto.Capacity
                };
                await _venueService.UpdateAsync(venue);
                TempData["Success"] = "Venue updated.";
            }

            return RedirectToAction("ManageVenues");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            var events = await _eventService.GetAllEventsAsync();
            var relatedEvents = events.Where(e => e.VenueId == id).ToList();

            foreach (var ev in relatedEvents)
            {
                await _eventService.DeleteEventAsync(ev.Id);
            }

            await _venueService.DeleteAsync(id);
            TempData["Success"] = "Venue and related events deleted.";
            return RedirectToAction("ManageVenues");
        }
    }
}

