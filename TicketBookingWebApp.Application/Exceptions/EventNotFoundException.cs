using System;

namespace TicketBookingWebApp.Application.Exceptions
{
    public class EventNotFoundException : Exception
    {
        public EventNotFoundException(string message) : base(message) { }
    }
}
