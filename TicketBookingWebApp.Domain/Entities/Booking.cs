using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketBookingWebApp.Domain.Entities;

public partial class Booking
{
    [Key]
    public int BookingId { get; set; }

    public int EventId { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    [StringLength(200)]
    public string EventTitle { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime EventDateTime { get; set; }

    public bool IsSeatBased { get; set; }

    public int Quantity { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime BookingDate { get; set; }

    public string? SeatIds { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ReferenceNumber { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("Bookings")]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Bookings")]
    public virtual User User { get; set; } = null!;
}
