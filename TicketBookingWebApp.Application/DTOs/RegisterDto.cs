using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBookingWebApp.Application.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(3, ErrorMessage = "Password must be at least 3 characters.")]
        //[RegularExpression(@"^(?=.*[!@#$%^&*(),.?""{}|<>]).+$", ErrorMessage = "Password must contain at least one special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
