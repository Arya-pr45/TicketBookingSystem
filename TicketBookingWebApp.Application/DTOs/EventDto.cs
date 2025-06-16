using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Application.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int VenueId { get; set; }
        public int TotalTickets { get; set; }
        public decimal Price { get; set; }

        public bool IsSeatBased { get; set; }
        public int AvailableTickets { get; set; }
        public int Quantity { get; set; }
        public List<Seat> Seats { get; set; } = new();
        public DateTime EventDateTime { get; set; }
        public VenueDto? Venue { get; set; }
    }
}
