using System;
using System.Collections.Generic;

namespace TicketBookingWebApp.Application.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }               
        public int EventId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDateTime { get; set; }

        public bool IsSeatBased { get; set; }           
        public int Quantity { get; set; }            

        public List<int> SeatIds { get; set; } = new();  
        public List<int> AvailableSeatIds { get; set; } = new(); 
    }
}
