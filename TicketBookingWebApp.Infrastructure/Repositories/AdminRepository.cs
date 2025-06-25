using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Domain.Entities;
using TicketBookingWebApp.Infrastructure.Interfaces;

namespace TicketBookingWebApp.Infrastructure.Repositories
{
    public class AdminRepository:IAdminRepository
    {
        private readonly TicketBookingSystemContext _context; //_context is EF Core DbContext tat handles communication with the database Here we inject DbContext for database access

        public AdminRepository(TicketBookingSystemContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users
                .Include(u => u.Bookings)
                .FirstOrDefaultAsync(u => u.UserName == userName); //returns the username or null
        }

    }
}
