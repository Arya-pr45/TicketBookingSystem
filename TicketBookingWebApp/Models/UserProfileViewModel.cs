namespace TicketBookingWebApp.Models
{

    public class UserProfileViewModel
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int TotalBookings { get; set; }
    }

}



