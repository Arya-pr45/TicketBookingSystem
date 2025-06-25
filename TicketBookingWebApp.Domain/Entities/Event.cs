using System;
using System.Collections.Generic;

namespace TicketBookingWebApp.Domain.Entities;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime EventDateTime { get; set; }

    public int VenueId { get; set; }

    public int TotalTickets { get; set; }

    public int AvailableTickets { get; set; }

    public bool IsSeatBased { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ImageUrl { get; set; }

    public int EventType { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual Venue Venue { get; set; } = null!;
}
