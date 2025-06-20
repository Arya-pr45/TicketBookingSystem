using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingWebApp.Application.DTOs;

namespace TicketBookingWebApp.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendBookingConfirmationEmailAsync(BookingDto booking);
    }

}
