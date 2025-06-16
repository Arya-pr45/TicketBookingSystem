using System;
using System.Collections.Generic;

namespace TicketBookingWebApp.Domain.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public int EventId { get; set; }

    public string Username { get; set; } = null!;

    public string EventTitle { get; set; } = null!;

    public DateTime EventDateTime { get; set; }

    public bool IsSeatBased { get; set; }

    public int Quantity { get; set; }

    public int UserId { get; set; }

    public DateTime BookingDate { get; set; }

    public string? SeatIds { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
