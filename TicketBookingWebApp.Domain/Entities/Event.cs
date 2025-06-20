using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketBookingWebApp.Domain.Entities;

public partial class Event
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EventDateTime { get; set; }

    public int VenueId { get; set; }

    public int TotalTickets { get; set; }

    public int AvailableTickets { get; set; }

    public bool IsSeatBased { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public int EventType { get; set; }

    [InverseProperty("Event")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Event")]
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    [ForeignKey("VenueId")]
    [InverseProperty("Events")]
    public virtual Venue Venue { get; set; } = null!;
}
