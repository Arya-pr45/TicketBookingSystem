using TicketBookingWebApp.Application.DTOs;

namespace TicketBookingWebApp.Web.ViewModels
{
    public class VenueManagementViewModel
    {
        public List<VenueDto> Venues { get; set; } = new();
        public VenueDto Venue { get; set; } = new();
        public bool IsEditMode => Venue != null && Venue.Id > 0;
    }

}
