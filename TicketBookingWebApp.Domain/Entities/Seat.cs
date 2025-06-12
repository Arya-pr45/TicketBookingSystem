using System;
using System.Collections.Generic;

namespace TicketBookingWebApp.Domain.Entities;

public partial class Seat
{
    public int Id { get; set; }

    public string SeatNumber { get; set; } = null!;

    public int VenueId { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Venue Venue { get; set; } = null!;
}
