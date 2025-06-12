using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Domain.Entities;

public class UserRepository : IUserRepository
{
    private readonly TicketBookingWebAppContext _context;

    public UserRepository(TicketBookingWebAppContext context)
    {
        _context = context;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == username);
    }
}
