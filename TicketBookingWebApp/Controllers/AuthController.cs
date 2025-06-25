using Microsoft.AspNetCore.Mvc;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;

namespace TicketBookingWebApp.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult RegisterUser()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                var role = User.IsInRole("Admin") ? "admin" : "user";

                if (role == "admin")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Event");
            }

            return View(new RegisterDto());
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);
                dto.Role = "User";
                var result = await _authService.RegisterAsync(dto);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(dto);
                }

                TempData["Message"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred during registration: {ex.Message}");
                return View(dto);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                var role = User.IsInRole("Admin") ? "admin" : "user"; 

                if (role == "admin")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Event");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var result = await _authService.LoginAsync(dto);

                if (!result.Success)
                {
                    var errorMessage = result.Message ?? "An unexpected error occurred.";
                    ModelState.AddModelError("", errorMessage);
                    return View(dto);
                }

                if (!string.IsNullOrEmpty(result.Token))
                {
                    Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });
                }

                var role = result.Role?.ToLower();
                if (role == "admin")
                    return RedirectToAction("Index", "Admin");

                if (role == "user")
                    return RedirectToAction("Index", "Event");

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred during login: {ex.Message}");
                return View(dto);
            }
        }


        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("AuthToken");
                TempData["Message"] = "You have been logged out.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while logging out: {ex.Message}";
                return RedirectToAction("Login");
            }
        }

    }
}