using System;

namespace TicketBookingWebApp.Application.Exceptions
{
    public class SeatUnavailableException : Exception
    {
        public SeatUnavailableException(string message) : base(message) { }
    }
}
