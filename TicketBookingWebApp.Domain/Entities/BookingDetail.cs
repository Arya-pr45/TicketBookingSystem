using System;
using System.Collections.Generic;

namespace TicketBookingWebApp.Domain.Entities;

public partial class BookingDetail
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int? SeatId { get; set; }

    public int? Quantity { get; set; }

    public string TicketType { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual Seat? Seat { get; set; }
}
