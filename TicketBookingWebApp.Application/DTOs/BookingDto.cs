using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Application.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }               
        public int EventId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDateTime { get; set; }

        public bool IsSeatBased { get; set; }           
        public int Quantity { get; set; }
        public DateTime BookingDate { get; set; }
      
        public string? ReferenceNumber { get; set; }

        public List<int> SeatIds { get; set; } = new();  
    }
}
