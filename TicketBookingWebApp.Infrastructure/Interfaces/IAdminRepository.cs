using TicketBookingWebApp.Domain.Entities;

namespace TicketBookingWebApp.Infrastructure.Interfaces
{
    public interface IAdminRepository
    {
        Task<User?> GetUserByUserNameAsync(string userName);

    }
}
