using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.Interfaces;
using TicketBookingWebApp.Application.Services;
using TicketBookingWebApp.Models;
using TicketBookingWebApp.Models;

namespace TicketBookingWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAdminService _adminService;


    public HomeController(ILogger<HomeController> logger,IAdminService adminService)
    {
        _logger = logger;
        _adminService = adminService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName)) return NotFound();

        var user = await _adminService.GetUserProfileAsync(userName);
        if (user == null) return NotFound();

        var viewModel = new UserProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role,
            TotalBookings = user.Bookings.Count
        };

        return View(viewModel);
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
