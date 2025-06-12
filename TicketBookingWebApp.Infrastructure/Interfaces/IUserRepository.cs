using Microsoft.EntityFrameworkCore;
using TicketBookingWebApp.Domain.Entities;

public interface IUserRepository
{
    Task<User> GetByUsernameAsync(string username);
}


