using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBookingWebApp.Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email must be a valid format (e.g., name@example.com).")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(3, ErrorMessage = "Password must be at least 3 characters.")]
        //[RegularExpression(@"^(?=.*[!@#$%^&*(),.?""{}|<>]).+$", ErrorMessage = "Password must contain at least one special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
