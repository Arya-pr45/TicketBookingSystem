using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Domain.Entities;

public class UserRepository : IUserRepository
{
    private readonly TicketBookingSystemContext _context;

    public UserRepository(TicketBookingSystemContext context)
    {
        _context = context;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == username);
    }
}
