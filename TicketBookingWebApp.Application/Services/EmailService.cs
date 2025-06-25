using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TicketBookingWebApp.Application.DTOs;
using TicketBookingWebApp.Application.Interfaces;
using System.Text;

public class EmailService : IEmailService
{
    private readonly SmtpDto _smtpSettings;

    public EmailService(IOptions<SmtpDto> smtpSettings) //IOptions will get the confirgutations from appsettings.json
    {
        _smtpSettings = smtpSettings.Value;// Here .Value will fetch actual value from teh configuration
    }

    public async Task SendBookingConfirmationEmailAsync(BookingDto booking)
    {
        var message = new MimeMessage(); // Mimemessage is class whch displays an email
        message.From.Add(new MailboxAddress("Ticket Booking", _smtpSettings.Username));
        message.To.Add(new MailboxAddress(booking.Username, booking.Email));
        message.Subject = "Booking Confirmation";

        var bodyText = new StringBuilder();
        bodyText.AppendLine($"Dear {booking.Username},");
        bodyText.AppendLine();
        bodyText.AppendLine("Your booking has been confirmed successfully!");
        bodyText.AppendLine();
        bodyText.AppendLine($"Booking ID: {booking.BookingId}");
        bodyText.AppendLine($"Event: {booking.EventTitle}");
        bodyText.AppendLine($"Date & Time: {booking.EventDateTime}");
        bodyText.AppendLine($"Quantity: {booking.Quantity}");

        if (booking.IsSeatBased && booking.SeatIds != null && booking.SeatIds.Any())
        {
            bodyText.AppendLine($"Seats: {string.Join(", ", booking.SeatIds)}"); // It will display the seatNumbers in the mail
        }

        bodyText.AppendLine($"Booking Date: {booking.BookingDate}");
        bodyText.AppendLine();
        bodyText.AppendLine("Thank you for booking with us!");
        bodyText.AppendLine("TicketBookingWebApp Team");

        var bodyBuilder = new BodyBuilder { TextBody = bodyText.ToString() };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient(); // using ensures client is disposed after the email was recieved , this manages the unmanaged resource and optimizes performance
        await client.ConnectAsync(_smtpSettings.Host, int.Parse(_smtpSettings.Port), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
