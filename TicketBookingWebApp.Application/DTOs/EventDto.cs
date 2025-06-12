using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<int> AvailableSeatIds { get; set; } = new List<int>();
        public DateTime EventDateTime { get; set; }
    }
}
