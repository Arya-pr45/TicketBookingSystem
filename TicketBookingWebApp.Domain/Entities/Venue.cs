using System;
using System.Collections.Generic;

namespace TicketBookingWebApp.Domain.Entities;

public partial class Venue
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public int Capacity { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
