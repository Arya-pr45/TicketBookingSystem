using System;
using System.Collections.Generic;

namespace TicketBookingWebApp.Domain.Entities;

public partial class Seat
{
    public int Id { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public bool IsBooked { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;
}
