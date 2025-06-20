using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketBookingWebApp.Domain.Entities;

public partial class Seat
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string SeatNumber { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public bool IsBooked { get; set; }

    public int EventId { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("Seats")]
    public virtual Event Event { get; set; } = null!;
}
